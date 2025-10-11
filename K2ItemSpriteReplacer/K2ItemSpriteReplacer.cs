using Asuna.Dialogues;
using Modding;
using UnityEngine;

namespace K2ItemSpriteReplacer
{
    public class TCModExample : ITCMod
    {
        public void OnDialogueStarted(Dialogue dialogue)
        {
            Debug.Log("Modded OnDialogueStarted");
        }

        public void OnFrame(float deltaTime)
        {

        }

        public void OnLevelChanged(string oldLevel, string newLevel)
        {
            Debug.Log("Modded OnLevelChanged");
        }

        public void OnLineStarted(DialogueLine line)
        {
            Debug.Log("Modded OnLineStarted");
        }

        public void OnModLoaded(ModManifest manifest)
        {
            Debug.Log("Modded OnModLoaded");
        }

        public void OnModUnLoaded()
        {
            Debug.Log("Modded OnModUnLoaded");
        }
    }
}
