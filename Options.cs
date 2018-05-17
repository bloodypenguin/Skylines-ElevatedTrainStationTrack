﻿
using MetroOverhaul.Detours;
using MetroOverhaul.OptionsFramework.Attibutes;

namespace MetroOverhaul
{
    [Options("MetroOverhaul")]
    public class Options
    {
        private const string UNSUBPREP = "Unsubscribe Prep";
        private const string STYLES = "Additional styles";
        private const string GENERAL = "General settings";
        public Options()
        {
            improvedPassengerTrainAi = true;
            improvedMetroTrainAi = true;
            metroUi = true;
            ghostMode = false;
        }
        [Checkbox("Metro track customization UI (requires reloading from main menu)", GENERAL)]
        public bool metroUi { set; get; }

        [Checkbox("Improved PassengerTrainAI (Allows trains to return to depots)", GENERAL, nameof(PassengerTrainAIDetour), nameof(PassengerTrainAIDetour.ChangeDeployState))]
        public bool improvedPassengerTrainAi { set; get; }

        [Checkbox("Improved MetroTrainAI (Allows trains to properly spawn at surface)", GENERAL, nameof(MetroTrainAIDetour), nameof(MetroTrainAIDetour.ChangeDeployState))]
        public bool improvedMetroTrainAi { set; get; }

        [Checkbox("GHOST MODE (Prior to unsubscribing this mod, all affected cities must be saved with this option on. OTHERWISE KEEP IT OFF)", UNSUBPREP)]
        public bool ghostMode { set; get; }
    }
}