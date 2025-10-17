using ANToolkit.Controllers;
using ANToolkit.ResourceManagement;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using HutongGames.PlayMaker;
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
            string temp = "Item list: \n";
            foreach (var item in Apparel.All)
            {
                if (item.Value is Apparel)
                {
                    if (item.Value.Name == "Vibrosuit")
                    {
                        Apparel apparel = (Apparel)item.Value;
                        temp += apparel.Name + "\n";
                        foreach (var durabilityDisplayLayers in apparel.DurabilityDisplayLayers)
                        {
                            temp += "----------------------------------------------------------------------------------------------------\n";
                            temp += "Name                  :" + durabilityDisplayLayers.Name + "\n";
                            temp += "AllowRecolor          :" + durabilityDisplayLayers.AllowRecolor + "\n";
                            temp += "Character             :" + durabilityDisplayLayers.Character + "\n";
                            temp += "DefaultColor          :" + durabilityDisplayLayers.DefaultColor + "\n";
                            temp += "DisableRestraint      :" + durabilityDisplayLayers.DisableRestraint + "\n";
                            temp += "DurabilitySprites     :" + durabilityDisplayLayers.DurabilitySprites + "\n";
                            temp += "Equipment             :" + durabilityDisplayLayers.Equipment + "\n";
                            temp += "InheritColorFromLayer :" + durabilityDisplayLayers.InheritColorFromLayer + "\n";
                            temp += "Initialized           :" + durabilityDisplayLayers.Initialized + "\n";
                            temp += "OnColorChanged        :" + durabilityDisplayLayers.OnColorChanged + "\n";
                            temp += "OnEnableChanged       :" + durabilityDisplayLayers.OnEnableChanged + "\n";
                            temp += "OnSpriteChanged       :" + durabilityDisplayLayers.OnSpriteChanged + "\n";
                            temp += "RequiresOpenTop       :" + durabilityDisplayLayers.RequiresOpenTop + "\n";
                            temp += "SortingLayer          :" + durabilityDisplayLayers.SortingLayer + "\n";
                            temp += "SortingOrder          :" + durabilityDisplayLayers.SortingOrder + "\n";
                            temp += "SpriteResource        :" + durabilityDisplayLayers.SpriteResource + "\n";
                            temp += "UseDurability         :" + durabilityDisplayLayers.UseDurability + "\n";
                            temp += "----------------------------------------------------------------------------------------------------\n";
                            temp += "\n";
                        }
                        foreach (var i2 in apparel.GetCustomFunctions())
                        {
                            temp += i2.ID + "\n";
                            
                        }
                        temp += "\n";
                    }
                }
            }
            Debug.Log(temp);
            File.WriteAllText(Path.Combine(manifest.ModPath, "data\\vibrosuit.txt"), temp);
            Debug.Log("K2-ItemSpriteReplacer Installed");
            CorrectSprite(manifest);
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
        public void CorrectSprite(ModManifest manifest) 
        {
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

                            ItemCustomFunction itemCustomFunction = new ItemCustomFunction
                            {
                                Hidden = false,
                                ID = "open_top2",
                                DisplayName = "Little Nakey Button",
                                RequiredStatAmount = 0,
                                RequiredStatID = "stat_crit_chance"
                            };

                            clone.AddCustomFunction(itemCustomFunction);

                            //clone.UseCustomFunction(Character.Get("Jenna"), "little_nakey_button", true);

                            var layer = clone.DurabilityDisplayLayers[0];
                            if (item.DisplaySprite != null)
                            {
                                layer.DisplaySprite = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.DisplaySprite));
                            }
                            if (item.PreviewSpritePath != null)
                            {
                                clone.DisplaySpriteResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.PreviewSpritePath));
                            }
                            if (item.IntactResourcePath != null && item.DamagedResourcePath != null && item.RuinedResourcePath != null && item.DestroyedResourcePath != null)
                            {
                                layer.DurabilitySprites.IntactResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.IntactResourcePath));
                                layer.DurabilitySprites.DamagedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.DamagedResourcePath));
                                layer.DurabilitySprites.RuinedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.RuinedResourcePath));
                                layer.DurabilitySprites.DestroyedResource = manifest.SpriteResolver.ResolveAsResource(Path.Combine(manifest.ModPath, item.DestroyedResourcePath));
                            }
                        }
                    }
                }
            });
        }
    }
    public class ItemSpriteInfo
    {
        public string Name { get; set; }
        public string DisplaySprite { get; set; }
        public string PreviewSpritePath { get; set; }
        public string IntactResourcePath { get; set; }
        public string DamagedResourcePath { get; set; }
        public string RuinedResourcePath { get; set; }
        public string DestroyedResourcePath { get; set; }
    }
}