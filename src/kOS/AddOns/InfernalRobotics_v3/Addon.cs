using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Suffixed.Part;

namespace kOS.AddOns.InfernalRobotics_v3
{
    [kOSAddon("IR3")]
    [kOS.Safe.Utilities.KOSNomenclature("IR3Addon")]
    public class Addon : Suffixed.Addon
    {
        public Addon(SharedObjects shared)
            : base(shared)
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("GROUPS", new Suffix<ListValue>(GetServoGroups, "List all ServoGroups"));
            AddSuffix("ALLSERVOS", new Suffix<ListValue>(GetAllServos, "List all Servos"));
            AddSuffix("PARTSERVOS", new OneArgsSuffix<ListValue, PartValue>(GetPartServos, "List Servos from Part"));
        }

        private ListValue GetServoGroups()
        {
            var list = new ListValue();

            if (!IR3Wrapper.APIReady)
            {
                throw new KOSUnavailableAddonException("IR3:GROUPS", "Infernal Robotics_v3");
            }

            var controlGroups = IR3Wrapper.IR3Controller.ServoGroups;

            if (controlGroups == null)
            {
                //Control Groups are somehow null, just return the empty list
                return list;
            }

            foreach (IR3Wrapper.IControlGroup cg in controlGroups)
            {
                if (cg.Vessel == null || cg.Vessel == shared.Vessel)
                    list.Add(new IRControlGroupWrapper(cg, shared));
            }

            return list;
        }

        private ListValue GetAllServos()
        {
            var list = new ListValue();

            if (!IR3Wrapper.APIReady)
            {
                throw new KOSUnavailableAddonException("IR3:ALLSERVOS", "Infernal Robotics_v3");
            }

            var controlGroups = IR3Wrapper.IR3Controller.ServoGroups;

            if (controlGroups == null)
            {
                //Control Groups are somehow null, just return the empty list
                return list;
            }

            foreach (IR3Wrapper.IControlGroup cg in controlGroups)
            {
                if (cg.Servos == null || (cg.Vessel != null && cg.Vessel != shared.Vessel))
                    continue;

                foreach (IR3Wrapper.IServo s in cg.Servos)
                {
                    list.Add(new IR3ServoWrapper(s, shared));
                }
            }

            return list;
        }

        private ListValue GetPartServos(PartValue pv)
        {
            var list = new ListValue();

            if (!IR3Wrapper.APIReady)
            {
                throw new KOSUnavailableAddonException("IR3:PARTSERVOS", "Infernal Robotics_v3");
            }

            var controlGroups = IR3Wrapper.IR3Controller.ServoGroups;

            if (controlGroups == null)
            {
                //Control Groups are somehow null, just return the empty list
                return list;
            }

            foreach (IR3Wrapper.IControlGroup cg in controlGroups)
            {
                if (cg.Servos == null || (cg.Vessel != null && cg.Vessel != shared.Vessel))
                    continue;

                foreach (IR3Wrapper.IServo s in cg.Servos)
                {
                    if (s.UID == pv.Part.craftID)
                        list.Add(new IR3ServoWrapper(s, shared));
                }
            }

            return list;
        }

        public override BooleanValue Available()
        {
            return IR3Wrapper.APIReady;
        }
    }
}