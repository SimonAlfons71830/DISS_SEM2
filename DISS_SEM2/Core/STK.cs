﻿using DISS_SEM2.Generators;
using DISS_SEM2.Objects;
using DISS_SEM2.Objects.Cars;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DISS_SEM2.Core
{
    public class STK : EventCore
    {
        private bool fixedSeed = false;

        public int todaysCustomers;

        List<ISTKObserver<STK>> _observers = new List<ISTKObserver<STK>>();
        private int speed;
        private double frequency;
        private int mode;
        //objectList - cakajuci zakaznici
        //should be queue
        public SimplePriorityQueue<Customer, double> customersLineQ;
        private SimplePriorityQueue<Customer, double> paymentLineQ;

        public List<Technician> technicians;
        public List<Automechanic> automechanics;
        //parkovacie miesta v dielni, ako list sa lepsie pristupuje k ukladaniu objektu na prve volne miesto
        private List<Customer> garageParkingSpace;
        private SimplePriorityQueue<Customer, double> garageParkingSpaceQ;
        //parkovacie miesta pred dielnou
        private List<Customer> parkingLot;
        public List<ParkingSpace> garage;

        //generator nasad
        private SeedGenerator seedGenerator;
        /// <summary>
        /// generator exponencialneho rozdelenia prichodu zakaznikov
        /// </summary>
        public Exponential customerArrivalTimeGenerator { get; set; }
        public Triangular takeOverTimeGenerator { get; set; }
        public ContinuousEven paymentTimeGenerator { get; set; }
        public DiscreteEven personalCarInspectionGenerator { get; set; }
        public EmpiricalDistribution vanCarInspectionGenerator { get; set; }
        public EmpiricalDistribution cargoCarInspectionGenerator { get; set; }
        public CarGenerator carTypeGenerator { get; set; }
        //boolean obsluhuje
        public bool obsluhuje { get; set; }
        public int CarsCountGarage { get; set; }
        public int _ids;
        private int _simulationTime;
        //cas zmeny
        public double timeOfLastChange;
        public int customerscount;

        public Statistics localAverageCustomerTimeInSTK;
        public Statistics localAverageTimeToTakeOverCar;
        public WeightedStatistics localAverageCustomerCountInLineToTakeOver;
        public WeightedStatistics localAverageFreeTechnicianCount;
        public WeightedStatistics localAverageFreeAutomechanicCount;
        public WeightedStatistics localAverageCustomerCountInSTK;
        public Statistics localAverageCustomerCountEndOfDay;

        public Statistics globalAverageCustomerTimeInSTK;
        public Statistics globalAverageTimeToTakeOverCar;
        public Statistics globalAverageCustomerCountInLineToTakeOver;
        public Statistics globalAverageFreeTechnicianCount;
        public Statistics globalAverageFreeAutomechanicCount;
        public Statistics globalAverageCustomerCountEndOfDay;
        public Statistics globalAverageCustomerCountInSTK;

        public STK()
        {
            customerscount = 0;
            todaysCustomers = 0;

            localAverageCustomerCountInSTK = new WeightedStatistics();
            localAverageCustomerTimeInSTK = new Statistics();
            localAverageTimeToTakeOverCar = new Statistics();
            localAverageCustomerCountInLineToTakeOver = new WeightedStatistics();
            localAverageFreeTechnicianCount = new WeightedStatistics();
            localAverageFreeAutomechanicCount = new WeightedStatistics();
            localAverageCustomerCountEndOfDay = new Statistics();

            globalAverageCustomerCountInSTK = new Statistics();
            globalAverageCustomerTimeInSTK = new Statistics();
            globalAverageTimeToTakeOverCar = new Statistics();
            globalAverageCustomerCountInLineToTakeOver = new Statistics();
            globalAverageFreeTechnicianCount = new Statistics();
            globalAverageFreeAutomechanicCount = new Statistics();
            globalAverageCustomerCountEndOfDay = new Statistics();


            this.customersLineQ = new SimplePriorityQueue<Customer, double>();
            this.paymentLineQ = new SimplePriorityQueue<Customer, double>();
            this.garageParkingSpaceQ = new SimplePriorityQueue<Customer, double>();
            CarsCountGarage = 0;

            this.obsluhuje = false;
            this.seedGenerator = new SeedGenerator();
            double _mi = 3600.0 / 23.0;

            this.customerArrivalTimeGenerator = new Exponential(this.seedGenerator, _mi);
            this.takeOverTimeGenerator = new Triangular(this.seedGenerator, 180, 695, 431);
            //this.carGenerator = new Random(this.seedGenerator.generate_seed());
            this.technicians = new List<Technician>();
            this._ids = 0;

            this.automechanics = new List<Automechanic>();

            garageParkingSpace = new List<Customer>();
            parkingLot = new List<Customer>();

            paymentTimeGenerator = new ContinuousEven(65, 177, this.seedGenerator); //<65,177)
            personalCarInspectionGenerator = new DiscreteEven(31 * 60, 45 * 60, this.seedGenerator);
            (int, int, double)[] vanRanges = {
            (35*60, 37*60, 0.2),
            (38 * 60, 40 * 60, 0.35),
            (41*60, 47*60, 0.3),
            (48*60, 52*60, 0.15)
            };
            vanCarInspectionGenerator = new EmpiricalDistribution(vanRanges, this.seedGenerator);
            (int, int, double)[] cargoRanges = {
            (37*60, 42*60, 0.05),
            (43*60, 45*60, 0.1),
            (46*60, 47*60, 0.15),
            (48*60, 51*60, 0.4),
            (52*60, 55*60, 0.25),
            (56*60, 65*60, 0.05)
            };
            cargoCarInspectionGenerator = new EmpiricalDistribution(cargoRanges, this.seedGenerator);
            this.carTypeGenerator = new CarGenerator(this.seedGenerator);
            this.speed = 1;


            this.frequency = 1; //kazda sekunda 

            this.garage = new List<ParkingSpace>();
            for (int i = 0; i < 5; i++)
            {
                var parking = new ParkingSpace();
                parking._id = i + 1;
                this.garage.Add(parking);
            }
            _simulationTime = 0;
            mode = 1; //fast je 2 
        }

        public void addCustomerToLine(Customer _customer)
        {
            //this.customersLine.Add(_customer);
            this.customersLineQ.Enqueue(_customer, _customer.arrivalTime);
        }

        public void removeCustomerFromLine()
        {
            this.customersLineQ.Dequeue(); //removes customer with min priority

            //return this.customersLine.Remove(_customer);
        }

        //vymaze zakaznika z cakacieho radu a prida ho do radu na platenie
        public void addCustomerToPaymentLine(Customer _customer)
        {
            //this.paymentLine.Add(_customer);
            this.paymentLineQ.Enqueue(_customer, _customer.arrivalTime);
        }

        public void removeCustomerFromPaymentLine()
        {
            this.paymentLineQ.Dequeue();
            //return this.paymentLine.Remove(_customer);
        }

        /// <summary>
        /// returns customer from first place in list
        /// </summary>
        /// <returns></returns>
        public Customer getCustomerInLine()
        {
            return this.customersLineQ.First();

        }
        /// <summary>
        /// returns customer from first place in payment line
        /// </summary>
        /// <returns></returns>
        public Customer getCustomerInPaymentLine()
        {
            return paymentLineQ.First();
        }

        public int getCustomersCountInLine()
        {
            return this.customersLineQ.Count();

            //return this.customersLine.Count;
        }

        public int getCustomersCountInPaymentLine()
        {
            return this.paymentLineQ.Count;
            //return this.paymentLine.Count;
        }

        public void addTechnician()
        {
            this.technicians.Add(new Technician());
        }

        public void addAutomechanic()
        {
            this.automechanics.Add(new Automechanic());
        }

        public int getTechniciansCount()
        { return this.technicians.Count; }
        public int getAutomechanicsCount()
        { return this.automechanics.Count; }


        public Technician getAvailableTechnician()
        {
            for (int i = 0; i < this.technicians.Count; i++)
            {
                if (!this.technicians[i].obsluhuje)
                {
                    return this.technicians[i];
                }
            }
            return null;
        }

        public Automechanic getAvailableAutomechanic()
        {
            for (int i = 0; i < this.automechanics.Count; i++)
            {
                if (!this.automechanics[i].obsluhuje)
                {
                    return this.automechanics[i];
                }
            }
            return null;
        }

        public int getCarsCountInGarage()
        {
            return this.garageParkingSpaceQ.Count;
            //return this.garageParkingSpace.Count;
        }
        public void parkCarInGarage(Customer _customer_car)
        {
            this.garageParkingSpaceQ.Enqueue(_customer_car, _customer_car.arrivalTime);
            //this.garageParkingSpace.Add(_customer_car);
        }
        /// <summary>
        /// removes car what came first 
        /// </summary>
        /// <param name="_customer_car"></param>
        public void removeCarFromGarage()
        {
            this.garageParkingSpaceQ.Dequeue();
            //return this.garageParkingSpace.Remove(_customer_car);
        }

        public void parkCarInParkingLot(Customer _customer_car)
        {
            this.parkingLot.Add(_customer_car);
        }
        public bool removeCarFromParkingLot(Customer _customer_car)
        {
            return this.parkingLot.Remove(_customer_car);
        }

        public Customer getNextCarInGarage()
        {
            return this.garageParkingSpaceQ.First();

        }
        /// <summary>
        /// removes specified car from parking lot in front of dielna
        /// </summary>
        /// <param name="_car"></param>
        /// <returns></returns>
        public bool getCustomersCarFromParkingLot(Customer _customer_car)
        {
            return this.parkingLot.Remove(_customer_car);
        }

        public void addObserver(ISTKObserver<STK> _stkObserver)
        {
            _observers.Add(_stkObserver);
        }

        public void Notify()
        {
            foreach (var _observer in _observers)
            {
                _observer.refresh(this);
            }
        }

        public void sleepSim()
        {
            Thread.Sleep(this.replications);
        }

        public int ReturnSpeed()
        {
            return this.speed;
        }

        public void SetSpeed(int _speed)
        {
            this.speed = _speed;
        }

        public int getFreeTechnicianCount()
        {
            var count = 0;
            foreach (var technic in this.technicians)
            {
                if (!technic.obsluhuje)
                {
                    count++;
                }
            }
            return count;
        }
        public int getAllTechniciansCount()
        {
            return this.technicians.Count;
        }

        public int getFreeAutomechanicCount()
        {
            var count = 0;
            foreach (var mechanic in this.automechanics)
            {
                if (!mechanic.obsluhuje)
                {
                    count++;
                }
            }
            return count;
        }
        public int getAllAutomechanicCount()
        {
            return this.automechanics.Count;
        }

        public void setFrequency(int _frequency)
        {
            this.frequency = _frequency;
        }
        internal double getFrequency()
        {
            return this.frequency;
        }
        public void pomCarsCountPlus()
        {
            this.CarsCountGarage++;
        }
        public int returnPomCarsCount()
        {
            return this.CarsCountGarage;
        }
        public void pomCarsCountMinus()
        {
            this.CarsCountGarage--;
        }
        public int getAvailableTechnicianCount()
        {
            int numb = 0;
            for (int i = 0; i < this.technicians.Count; i++)
            {
                if (!this.technicians[i].obsluhuje)
                {
                    numb++;
                }
            }
            return numb;
        }

        public int getAvailableAutomechanicCount()
        {
            int numb = 0;
            for (int i = 0; i < this.automechanics.Count; i++)
            {
                if (!this.automechanics[i].obsluhuje)
                {
                    numb++;
                }
            }
            return numb;
        }
        public List<Technician> getTechnicianList()
        {
            return this.technicians;
        }
        public List<Automechanic> getAutomechanicsList()
        {
            return this.automechanics;
        }
        public void setId(Customer _customer)
        {
            _ids++;
            _customer._id = _ids;
        }
        public List<Customer> getCustomersInLine()
        {
            return this.customersLineQ.ToList();
        }

        public bool reserveParkingSpace(Customer customer_car)
        {
            var parkingSpace = this.getAvailableParkingSpace();
            if (parkingSpace != null)
            {
                parkingSpace.free = false;
                parkingSpace.parkedCar = customer_car;
                return true;
            }
            return false;
        }

        public bool isParkingFree()
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (this.garage[i].free)
                {
                    return true;
                }
            }
            return false;

        }
        public bool reserveParking()
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (this.garage[i].free)
                {
                    this.garage[i].free = false;
                    return true;
                }
            }
            return false;
        }
        public bool freeParking()
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (!this.garage[i].free)
                {
                    this.garage[i].free = true;
                    return true;
                }
            }
            return false;
        }

        public bool freeParkingSpace(Customer customer)
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (this.garage[i].parkedCar == customer)
                {
                    this.garage[i].parkedCar = null;
                    this.garage[i].free = true;
                    return true;
                }
            }
            return false;
        }

        public ParkingSpace getAvailableParkingSpace()
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (this.garage[i].free)
                {
                    return this.garage[i];
                }
            }
            return null;
        }
        public int getReserverParkingPlaces()
        {
            var number = 0;
            for (int i = 0; i < this.garage.Count; i++)
            {
                if (!this.garage[i].free)
                {
                    number++;
                }
            }
            return number;
        }

        public void createTechnicians(int number)
        {
            for (int i = 0; i < number; i++)
            {
                var technic = new Technician();
                technic._id = i;
                this.technicians.Add(technic);
            }
        }

        public void createAutomechanics(int number)
        {

            for (int i = 0; i < number; i++)
            {
                var automech = new Automechanic();
                automech._id = i;
                this.automechanics.Add(automech);
            }
        }

        public void setSimulationTime(int _time)
        {
            this._simulationTime = _time;
        }

        public int getSimulationTime()
        {
            return this._simulationTime;
        }

        public void setMode(int _number)
        {
            this.mode = _number;
        }

        public int getMode()
        {
            return this.mode;
        }
        /// <summary>
        /// priemerny cas zakaznika v celom systeme
        /// nesmie prekrocit 70 min
        /// </summary>
        /// <returns></returns>
        public double getStatI()
        {
            var sec = this.globalAverageCustomerTimeInSTK.getMean();
            var min = sec / 60;
            return min;
        }
        /// <summary>
        /// priemerny cas zakaznika v rade na odovzdanie auta 
        /// nesmie prekrocit 10 min
        /// </summary>
        /// <returns></returns>
        public double getStatII()
        {
            var sec = this.globalAverageTimeToTakeOverCar.getMean();
            var min = sec / 60;
            return min;
        }
        /// <summary>
        /// customers in waiting line count
        /// </summary>
        /// <returns></returns>
        public double getStatIII()
        {
            return this.globalAverageCustomerCountInLineToTakeOver.getMean(); ;
        }
        /// <summary>
        /// free technicians
        /// </summary>
        /// <returns></returns>
        public double getStatIV()
        {
            return this.globalAverageFreeTechnicianCount.getMean();
        }

        /// <summary>
        /// free automechanics
        /// </summary>
        /// <returns></returns>
        public double getStatV()
        {
            return this.globalAverageFreeAutomechanicCount.getMean();
        }
        public int getActualReplication()
        {
            return this.replications;
        }

        /// <summary>
        /// pocet zakaznikov v stk
        /// </summary>
        /// <returns></returns>
        public double getStatVI()
        {
            return this.globalAverageCustomerCountInSTK.getMean();
        }

        public double getStatVII()
        {
            return this.globalAverageCustomerCountEndOfDay.getMean();
        }

        public void resetGarage()
        {
            for (int i = 0; i < this.garage.Count; i++)
            {
                this.garage[i].free = true;
            }
        }

        public void resetTechnicians()
        {
            for (int i = 0; i < this.technicians.Count; i++)
            {
                this.technicians[i].obsluhuje = false;
            }
        }

        public void resetAutomechanics()
        {
            for (int i = 0; i < this.automechanics.Count; i++)
            {
                this.automechanics[i].obsluhuje = false;
            }
        }

        public void resetSim()
        {
            this.replications = 0;
            this.resetGarage();
            this.resetTechnicians();
            this.resetAutomechanics();
            this.resetQueues();
            this.localAverageCustomerCountInSTK.resetStatistic();
        }

        public void resetQueues()
        {
            this.customersLineQ.Clear();
            this.garageParkingSpaceQ.Clear();
            this.paymentLineQ.Clear();
            CarsCountGarage = 0;
            _ids = 0;
            _simulationTime = 0;
            //cas zmeny
            timeOfLastChange = 0;
            customerscount = 0;

        }

        public void endCustomersWaiting()
        {
            var pomcount = this.customersLineQ.Count;
            for (int i = 0; i < pomcount; i++)
            {
                var customerLeft = this.customersLineQ.Dequeue();
                var pomTime = currentTime;
                for (int j = 0; j < technicians.Count; j++)
                {
                    if (customerLeft == technicians[j].customer_car)
                    {
                        technicians[j].obsluhuje = false;
                        technicians[j].customer_car = null;
                    }
                }
                this.localAverageCustomerTimeInSTK.addValues(currentTime - customerLeft.arrivalTime);
            }
            var pomparkingspace = this.garageParkingSpaceQ.Count;
            for (int i = 0; i < pomparkingspace; i++)
            {
                var customerleft = this.garageParkingSpaceQ.Dequeue();
                var pomTime = currentTime;
                for (int j = 0; j < technicians.Count; j++)
                {
                    if (customerleft == technicians[j].customer_car)
                    {
                        technicians[j].obsluhuje = false;
                        technicians[j].customer_car = null;
                    }
                }
                this.localAverageCustomerTimeInSTK.addValues(currentTime - customerleft.arrivalTime);
            }
            var pompaymentline = this.paymentLineQ.Count;
            for (int i = 0; i < pompaymentline; i++)
            {
                var customerLeft = this.paymentLineQ.Dequeue();
                for (int j = 0; j < technicians.Count; j++)
                {
                    if (customerLeft == technicians[j].customer_car)
                    {
                        technicians[j].obsluhuje = false;
                        technicians[j].customer_car = null;
                    }
                }
                var pomTime = currentTime;
                this.localAverageCustomerTimeInSTK.addValues(pomTime - customerLeft.arrivalTime);
            }

            for (int i = 0; i < technicians.Count; i++)
            {
                if (technicians[i].obsluhuje)
                {
                    var pomcustomer = technicians[i].customer_car;
                    this.localAverageCustomerTimeInSTK.addValues(currentTime - pomcustomer.arrivalTime);
                }
            }
            for (int i = 0; i < automechanics.Count; i++)
            {
                if (automechanics[i].obsluhuje)
                {
                    var pomcustomer = automechanics[i].customer_car;
                    this.localAverageCustomerTimeInSTK.addValues(currentTime - pomcustomer.arrivalTime);
                }
            }

        }

        public int notFinishedCustomers()
        {
            return this.customerscount;
        }

        public void setFixedSeedSim(bool pom)
        {
            this.fixedSeed = pom;

        }

        public void generateNumbersAgain()
        {
            double _mi = 3600.0 / 23.0;

            this.customerArrivalTimeGenerator = new Exponential(this.seedGenerator, _mi);

            this.takeOverTimeGenerator = new Triangular(this.seedGenerator, 180, 695, 431);
            //this.carGenerator = new Random(this.seedGenerator.generate_seed());

            paymentTimeGenerator = new ContinuousEven(65, 177, this.seedGenerator); //<65,177)

            personalCarInspectionGenerator = new DiscreteEven(31 * 60, 45 * 60, this.seedGenerator);

            (int, int, double)[] vanRanges = {
            (35*60, 37*60, 0.2),
            (38 * 60, 40 * 60, 0.35),
            (41*60, 47*60, 0.3),
            (48*60, 52*60, 0.15)
            };
            vanCarInspectionGenerator = new EmpiricalDistribution(vanRanges, this.seedGenerator);

            (int, int, double)[] cargoRanges = {
            (37*60, 42*60, 0.05),
            (43*60, 45*60, 0.1),
            (46*60, 47*60, 0.15),
            (48*60, 51*60, 0.4),
            (52*60, 55*60, 0.25),
            (56*60, 65*60, 0.05)
            };
            cargoCarInspectionGenerator = new EmpiricalDistribution(cargoRanges, this.seedGenerator);

            this.carTypeGenerator = new CarGenerator(this.seedGenerator);
        }
        
    }

}
