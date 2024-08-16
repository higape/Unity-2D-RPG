using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Static;
using UnityEngine;

namespace Root
{
    /// <summary>
    /// 资源管理器，负责加载和提供资源。
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        public const string SaveFileName = "SAVEDATA??.save";

        public static string RootDirectory
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Test");
#else
                return Application.dataPath;
#endif
            }
        }

        public static string SaveDirectory => Path.Combine(RootDirectory, "Save");

        public static GameInfo GameInfo { get; private set; }
        public static Term Term { get; private set; }
        public static Spriteset Spriteset { get; set; }
        public static BattleEffectList BattleEffect { get; private set; }
        public static TraitList Trait { get; private set; }
        public static DurationStateList DurationState { get; private set; }
        public static WeaponUsageList WeaponUsage { get; private set; }
        public static ActorList Actor { get; private set; }
        public static ActorWeaponList ActorWeapon { get; private set; }
        public static ActorWeaponSkinList ActorWeaponSkin { get; private set; }
        public static ActorArmorList ActorHeadArmor { get; private set; }
        public static ActorArmorList ActorBodyArmor { get; private set; }
        public static ActorArmorList ActorHandArmor { get; private set; }
        public static ActorArmorList ActorFootArmor { get; private set; }
        public static ActorArmorList ActorOrnamentArmor { get; private set; }
        public static ActorUsableItemList ActorRecoverItem { get; private set; }
        public static ActorUsableItemList ActorAttackItem { get; private set; }
        public static ActorUsableItemList ActorAuxiliaryItem { get; private set; }
        public static ActorNormalItemList ActorNormalItem { get; private set; }
        public static SkillList Skill { get; private set; }
        public static EnemyList Enemy { get; private set; }
        public static ShopList Shop { get; private set; }

        public static ActorUsableItemList GetActorUsableItemList(ActorUsableItem.ItemType type) =>
            type switch
            {
                ActorUsableItem.ItemType.RecoverItem => ActorRecoverItem,
                ActorUsableItem.ItemType.AttackItem => ActorAttackItem,
                ActorUsableItem.ItemType.AuxiliaryItem => ActorAuxiliaryItem,
                _ => null,
            };

        public static ActorArmorList GetActorArmorList(int slotIndex) =>
            slotIndex switch
            {
                0 => ActorHeadArmor,
                1 => ActorBodyArmor,
                2 => ActorHandArmor,
                3 => ActorFootArmor,
                4 => ActorOrnamentArmor,
                _ => null,
            };

        public static string GetItemName(CommonItemType itemType, int itemID) =>
            itemType switch
            {
                CommonItemType.ActorNormalItem => ActorNormalItem.GetItem(itemID).Name,
                CommonItemType.ActorRecoverItem => ActorRecoverItem.GetItem(itemID).Name,
                CommonItemType.ActorAttackItem => ActorAttackItem.GetItem(itemID).Name,
                CommonItemType.ActorAuxiliaryItem => ActorAuxiliaryItem.GetItem(itemID).Name,
                CommonItemType.ActorWeapon => ActorWeapon.GetItem(itemID).Name,
                CommonItemType.ActorHeadArmor => ActorHeadArmor.GetItem(itemID).Name,
                CommonItemType.ActorBodyArmor => ActorBodyArmor.GetItem(itemID).Name,
                CommonItemType.ActorHandArmor => ActorHandArmor.GetItem(itemID).Name,
                CommonItemType.ActorFootArmor => ActorFootArmor.GetItem(itemID).Name,
                CommonItemType.ActorOrnamentArmor => ActorOrnamentArmor.GetItem(itemID).Name,
                _ => string.Empty,
            };

        /// <summary>
        /// 创建存档路径
        /// </summary>
        public static void CheckAndCreatePath()
        {
            try
            {
                string savePath = SaveDirectory;
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void SaveJson(string filepath, object data)
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(filepath, json, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static T LoadJson<T>(string filepath)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    string json = File.ReadAllText(filepath, Encoding.UTF8);
                    var obj = JsonUtility.FromJson<T>(json);
                    if (obj == null)
                        Debug.LogError("Failed to load:" + filepath);
                    return obj;
                }
                else
                {
                    Debug.LogError("Failed to load:" + filepath);
                    return default;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        public static FixedEncounter LoadFixedEncounter(string filename)
        {
            return Resources.Load<FixedEncounter>(Path.Combine("FixedEncounters", filename));
        }

        public static T LoadInternalData<T>(string filepath)
            where T : UnityEngine.Object
        {
            return Resources.Load<T>(filepath);
        }

        public static bool SaveNewFile(SaveData saveData)
        {
            try
            {
                var dirpath = SaveDirectory;
                if (!Directory.Exists(dirpath))
                    CheckAndCreatePath();
                string[] dirs = Directory.GetFiles(dirpath, SaveFileName);
                int i;
                for (i = 1; i < 100; i++)
                {
                    bool result = true;
                    string current = SaveFileName.Replace("??", i.ToString().PadLeft(2, '0'));
                    foreach (string dir in dirs)
                    {
                        string fn = Path.GetFileName(dir);
                        if (current == fn)
                        {
                            result = false;
                            break;
                        }
                    }
                    if (result)
                    {
                        saveData.name = current.Remove(10);
                        SaveJson(Path.Combine(dirpath, current), saveData);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return false;
        }

        public static bool SaveFile(SaveData saveData)
        {
            try
            {
                var dirpath = SaveDirectory;
                if (!Directory.Exists(dirpath))
                    CheckAndCreatePath();
                SaveJson(Path.Combine(dirpath, saveData.name + ".save"), saveData);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public static SaveData[] LoadSaveInfo()
        {
            try
            {
                string[] dirs = Directory.GetFiles(SaveDirectory, SaveFileName);
                List<SaveData> saveDatas = new();
                foreach (string dir in dirs)
                {
                    string fn = Path.GetFileName(dir);
                    if (char.IsDigit(fn[8]) && char.IsDigit(fn[9]))
                    {
                        var save = LoadJson<SaveData>(dir);
                        if (save != null)
                        {
                            save.name = fn.Remove(10);
                            saveDatas.Add(save);
                        }
                    }
                }
                return saveDatas.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        [SerializeField]
        private GameInfo gameInfo;

        [SerializeField]
        private Term term;

        [SerializeField]
        private Spriteset spriteset;

        [SerializeField]
        private BattleEffectList battleEffect;

        [SerializeField]
        private TraitList trait;

        [SerializeField]
        private DurationStateList durationState;

        [SerializeField]
        private WeaponUsageList weaponUsage;

        [SerializeField]
        private ActorList actor;

        [SerializeField]
        private ActorWeaponList actorWeapon;

        [SerializeField]
        private ActorWeaponSkinList actorWeaponSkin;

        [SerializeField]
        private ActorArmorList actorHeadArmor;

        [SerializeField]
        private ActorArmorList actorBodyArmor;

        [SerializeField]
        private ActorArmorList actorHandArmor;

        [SerializeField]
        private ActorArmorList actorFootArmor;

        [SerializeField]
        private ActorArmorList actorOrnamentArmor;

        [SerializeField]
        private ActorUsableItemList actorRecoverItem;

        [SerializeField]
        private ActorUsableItemList actorAttackItem;

        [SerializeField]
        private ActorUsableItemList actorAuxiliaryItem;

        [SerializeField]
        private ActorNormalItemList actorNormalItem;

        [SerializeField]
        private SkillList skill;

        [SerializeField]
        private EnemyList enemy;

        [SerializeField]
        private ShopList shop;

        private void Awake()
        {
            CheckAndCreatePath();
            GameInfo = gameInfo;
            Term = term;
            Spriteset = spriteset;
            BattleEffect = battleEffect;
            Trait = trait;
            DurationState = durationState;
            WeaponUsage = weaponUsage;
            Actor = actor;
            ActorWeapon = actorWeapon;
            ActorWeaponSkin = actorWeaponSkin;
            ActorHeadArmor = actorHeadArmor;
            ActorBodyArmor = actorBodyArmor;
            ActorHandArmor = actorHandArmor;
            ActorFootArmor = actorFootArmor;
            ActorOrnamentArmor = actorOrnamentArmor;
            ActorRecoverItem = actorRecoverItem;
            ActorAttackItem = actorAttackItem;
            ActorAuxiliaryItem = actorAuxiliaryItem;
            ActorNormalItem = actorNormalItem;
            Skill = skill;
            Enemy = enemy;
            Shop = shop;
        }

#if UNITY_EDITOR
        //生成在开发期间用于查找数据的文本，并写入剪贴板
        [ContextMenu("Make ItemList To String To CopyBuffer")]
        private void MakeText()
        {
            var sb = new StringBuilder();
            sb.Append(MakeText(battleEffect.ItemList));
            sb.Append(MakeText(trait.ItemList));
            sb.Append(MakeText(durationState.ItemList));
            sb.Append(MakeText(weaponUsage.ItemList));
            sb.Append(MakeText(skill.ItemList));
            sb.Append(MakeText(actor.ItemList));
            sb.Append(MakeText(actorWeapon.ItemList));
            sb.Append(MakeText(actorWeaponSkin.ItemList));
            sb.Append(MakeText(actorHeadArmor.ItemList));
            sb.Append(MakeText(actorBodyArmor.ItemList));
            sb.Append(MakeText(actorHandArmor.ItemList));
            sb.Append(MakeText(actorFootArmor.ItemList));
            sb.Append(MakeText(actorOrnamentArmor.ItemList));
            sb.Append(MakeText(actorRecoverItem.ItemList));
            sb.Append(MakeText(actorAttackItem.ItemList));
            sb.Append(MakeText(actorAuxiliaryItem.ItemList));
            sb.Append(MakeText(actorNormalItem.ItemList));
            sb.Append(MakeText(enemy.ItemList));
            sb.Append(MakeText(shop.ItemList));
            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        private StringBuilder MakeText(System.Collections.IList list)
        {
            var sb = new StringBuilder();
            sb.Append(list.GetType().Name);
            sb.Append('\n');
            foreach (var item in list)
            {
                if (item is NameItem nameItem)
                {
                    sb.Append(nameItem.id.ToString().PadLeft(10));
                    sb.Append(':');
                    sb.Append(nameItem.Name);
                    sb.Append('\n');
                }
            }
            sb.Append('\n');
            return sb;
        }
#endif
    }
}
