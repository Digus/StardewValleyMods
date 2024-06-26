﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

namespace PFMAutomate.Automate
{
    internal class CustomClonerMachine : IMachine
    {
        public string MachineTypeID { get; }

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }

        private readonly SObject _machine;

        public IMachine ClonerMachine;
        public CustomProducerMachine PfmMachine;

        public CustomClonerMachine(IMachine clonerMachine)
        {
            Location = clonerMachine.Location;
            TileArea = clonerMachine.TileArea;
            this.ClonerMachine = clonerMachine;
            _machine = PFMAutomateModEntry.Helper.Reflection.GetField<SObject>(clonerMachine, "Machine").GetValue();
            this.PfmMachine = new CustomProducerMachine(_machine, clonerMachine.Location, _machine.TileLocation);
            MachineTypeID = "PFM.Cloner." + _machine.Name;
        }

        public MachineState GetState()
        {
            var machineState = this.PfmMachine.GetState();
            return machineState == MachineState.Empty ? ClonerMachine.GetState() : machineState;
        }

        public ITrackedStack GetOutput()
        {
            this.PfmMachine.GetOutput();
            ITrackedStack trackedStack = this.ClonerMachine.GetOutput();
            this.PfmMachine.Reset(_machine.heldObject.Value);
            return trackedStack;
        }

        public bool SetInput(IStorage input)
        {
            return this.PfmMachine.SetInput(input) || this.ClonerMachine.SetInput(input);
        }
    }
}
