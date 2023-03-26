﻿using DISS_SEM2.Generators;
using DISS_SEM2.Objects.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Core
{
    public class STK : EventCore
    {
        //objectList - cakajuci zakaznici

        private List<Customer> customersLine{ get; set; }
        private List<Customer> paymentLine { get; set; }
        private List<Technician> technicians;
        private List<Automechanic> automechanics;
        //parkovacie miesta v dielni, ako list sa lepsie pristupuje k ukladaniu objektu na prve volne miesto
        private List<Car> garageParkingSpace;
        //parkovacie miesta pred dielnou
        private List<Car> parkingLot;

        //generator nasad
        private SeedGenerator seedGenerator;
        /// <summary>
        /// generator exponencialneho rozdelenia prichodu zakaznikov
        /// </summary>
        public Exponential customerArrivalTimeGenerator { get; set; }
        public Triangular takeOverTimeGenerator { get; set; }
        public ContinuousEven paymentTimeGenerator { get; set; }
        //boolean obsluhuje
        public bool obsluhuje { get; set; }

        Random carGenerator;
        //cas zmeny
        public double timeOfLastChange;

        public STK()
        {
            this.obsluhuje = false;
            this.customersLine = new List<Customer>();
            this.paymentLine = new List<Customer>();
            this.seedGenerator = new SeedGenerator();
            double _mi = 60 / 23;
            this.customerArrivalTimeGenerator = new Exponential(this.seedGenerator, _mi);
            this.takeOverTimeGenerator = new Triangular(this.seedGenerator, 180, 695, 431);
            //??
            this.carGenerator = new Random(this.seedGenerator.generate_seed());
            this.technicians = new List<Technician>();

            for (int i = 0; i < 5; i++)
            {
                this.technicians.Add(new Technician());
            }

            this.automechanics = new List<Automechanic>();

            for (int i = 0; i < 5; i++)
            {
                this.automechanics.Add(new Automechanic());
            }

            garageParkingSpace = new List<Car>();
            parkingLot = new List<Car>();

            paymentTimeGenerator = new ContinuousEven(65, 177, this.seedGenerator); //<65,177)

        }

        public void addCustomerToLine(Customer _customer)
        {
            this.customersLine.Add(_customer);
        }

        public bool removeCustomerFromLine(Customer _customer)
        {
            return this.customersLine.Remove(_customer);
        }

        //vymaze zakaznika z cakacieho radu a prida ho do radu na platenie
        public bool addCustomerToPaymentLine(Customer _customer)
        {
            if (this.removeCustomerFromLine(_customer))
            {
                this.paymentLine.Add(_customer);
                return true;
            }
            //nepodarilo sa vymazat zakaznika z predchadzajuceho radu
            return false;
        }

        public bool removeCustomerFromPaymentLine(Customer _customer)
        { 
            return this.paymentLine.Remove(_customer);
        }

        /// <summary>
        /// returns customer from first place in list
        /// </summary>
        /// <returns></returns>
        public Customer getCustomerInLine()
        {
            if (this.customersLine.Count != 0)
            {
                var customer = this.customersLine[0];
                return customer;
            }
            return null;
        }
        /// <summary>
        /// returns customer from first place in payment line
        /// </summary>
        /// <returns></returns>
        public Customer getCustomerInPaymentLine()
        {
            if (this.paymentLine.Count != 0)
            {
                var customer = this.paymentLine[0];
                return customer;
            }
            return null;
        }

        public int getCustomersCountInLine()
        { 
            return this.customersLine.Count;
        }

        public int getCustomersCountInPaymentLine()
        { 
            return this.customersLine.Count;
        }

        public CarTypes generateCarType() 
        {
            var genNumber = carGenerator.Next();

            if ( genNumber < 0.65)
            {
                return CarTypes.Personal;
            }
            else if (genNumber < 86)
            {
                return CarTypes.Van;
            }
            else
            {
                return CarTypes.Cargo;
            }
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
        { return this.technicians.Count;}
        public int getAutomechanicsCount()
        { return this.automechanics.Count;}

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

        public int getFreeSpacesInGarage()
        {
            return this.garageParkingSpace.Count;
        }

        public void parkCarInGarage(Car _car)
        {
            this.garageParkingSpace.Add(_car);
        }
        public bool removeCarFromGarage(Car _car)
        {
            return this.garageParkingSpace.Remove(_car);
        }

        public void parkCarInParkingLot(Car _car)
        {
            this.parkingLot.Add(_car);
        }
        public bool removeCarFromParkingLot(Car _car)
        {
            return this.parkingLot.Remove(_car);
        }
    }
}