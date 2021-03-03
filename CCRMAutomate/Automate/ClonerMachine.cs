using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using CustomCrystalariumMod;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using SObject = StardewValley.Object;

namespace CCRMAutomate.Automate
{
    internal class ClonerMachine : IMachine
    {
        public string MachineTypeID { get; }

        public readonly SObject Machine;
        public GameLocation Location { get; }
        public Rectangle TileArea { get; }

        public ClonerMachine(SObject machine, GameLocation location, Vector2 tile)
        {
            MachineTypeID = "CCRM.Cloner." + machine.Name;
            Machine = machine;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            return this.Machine.heldObject.Value == null
                ? MachineState.Disabled
                : this.Machine.readyForHarvest.Value
                    ? MachineState.Done
                    : MachineState.Processing;
        }

        public ITrackedStack GetOutput()
        {
            SObject machine = this.Machine;
            SObject heldObject = machine.heldObject.Value;
            return new TrackedItem(heldObject.getOne(), item =>
            {
                CustomCloner cloner = ClonerController.GetCloner(machine.Name);
                int? machineMinutesUntilReady = null;
                if (cloner.CloningDataId.ContainsKey(item.ParentSheetIndex))
                {
                    machineMinutesUntilReady = cloner.CloningDataId[item.ParentSheetIndex];
                }
                else if (cloner.CloningDataId.ContainsKey(item.Category))
                {
                    machineMinutesUntilReady = cloner.CloningDataId[item.Category];
                }
                if (machineMinutesUntilReady.HasValue)
                {
                    machine.heldObject.Value = heldObject;
                    machine.MinutesUntilReady = machineMinutesUntilReady.Value;
                    machine.initializeLightSource(machine.TileLocation, false);
                }
                else
                {
                    machine.heldObject.Value = (SObject)null;
                    machine.MinutesUntilReady = -1;
                }
                machine.readyForHarvest.Value = false;
            });
        }

        public bool SetInput(IStorage input)
        {
            // started manually
            return false;
        }
    }
}
