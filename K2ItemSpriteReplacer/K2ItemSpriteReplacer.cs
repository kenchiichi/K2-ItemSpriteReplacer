using ANToolkit.Controllers;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Modding;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace K2ItemSpriteReplacer
{
    public class TCModExample : ITCMod
    {
        public void OnDialogueStarted(Dialogue dialogue) { }

        public void OnFrame(float deltaTime) { }

        public void OnLevelChanged(string oldLevel, string newLevel) { }

        public void OnLineStarted(DialogueLine line) { }

        public void OnModLoaded(ModManifest manifest)
        {
            Debug.Log("K2-ItemSpriteReplacer Installed");

            using StreamReader k2StreamReader = new StreamReader(Path.Combine(manifest.ModPath, "data\\ItemData.xml"));

            List<ItemSpriteInfo> k2Sprites = Deserialize<List<ItemSpriteInfo>>(k2StreamReader.ReadToEnd());

            Item.OnItemCloned.AddListener((newItem, oldItem) =>
            {
                if (newItem is Equipment && oldItem is Equipment)
                {
                    foreach (var item in k2Sprites)
                    {
                        if (oldItem.Name == item.Name)
                        {
                            Equipment clone = newItem as Equipment;
                            clone.DisplaySpriteResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.PreviewSpritePath));
                            clone.DurabilityDisplayLayers.ForEach(layer =>
                            {
                                layer.DurabilitySprites.IntactResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.IntactResourcePath));
                                layer.DurabilitySprites.RuinedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.DamagedResourcePath));
                                layer.DurabilitySprites.DamagedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.RuinedResourcePath));
                                layer.DurabilitySprites.DestroyedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.DestroyedResourcePath));
                            });
                        }
                    }
                }
            });
        }

        public void OnModUnLoaded()
        {
            Debug.Log("K2-ItemSpriteReplacer Uninstalled");
        }
        private static T Deserialize<T>(string xmlString)
        {
            if (xmlString == null) return default;
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xmlString);
            return (T)serializer.Deserialize(reader);
        }
    }
    public class ItemSpriteInfo
    {
        public string Name { get; set; }
        public string PreviewSpritePath { get; set; }
        public string IntactResourcePath { get; set; }
        public string DamagedResourcePath { get; set; }
        public string RuinedResourcePath { get; set; }
        public string DestroyedResourcePath { get; set; }
    }
}