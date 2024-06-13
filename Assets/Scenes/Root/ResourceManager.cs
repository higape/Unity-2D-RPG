using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Static;
using TMPro;
using UnityEngine;

namespace Root
{
    /// <summary>
    /// 资源管理器，负责加载和提供资源。
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
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

        public static GameInfo GameInfo { get; private set; }
        public static Term Term { get; private set; }
        public static BattleEffectList BattleEffect { get; private set; }
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

        public static bool CheckFolderPath(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public static bool CheckFilePath(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 创建存档路径
        /// </summary>
        public static void CheckAndCreatePath()
        {
            try
            {
                string savePath = Path.Combine(RootDirectory, "Save");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
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
                filepath = Path.Combine(RootDirectory, filepath);
                if (CheckFilePath(filepath))
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

        //存档
        /* public static bool SaveFile(string filename, object save)
        {
            try
            {
                string saveFolderPath = Path.Combine(RootDirectory, "Save");
                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                string filePath = Path.Combine(saveFolderPath, filename);
                BinaryFormatter bf = new();
                FileStream file = File.Create(filePath);
                bf.Serialize(file, save);
                file.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        } */

        //读档
        /* public static object LoadFile(string filename)
        {
            try
            {
                string filePath = Path.Combine(RootDirectory, "Save", filename);
                if (File.Exists(filePath))
                {
                    BinaryFormatter bf = new();
                    FileStream file = File.Open(filePath, FileMode.Open);
                    object save = bf.Deserialize(file);
                    file.Close();
                    return save;
                }
                else
                {
                    Debug.Log("No gamesaved!");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        } */

        [SerializeField]
        private GameInfo gameInfo;

        [SerializeField]
        private Term term;

        [SerializeField]
        private BattleEffectList battleEffect;

        [SerializeField]
        private DurationStateList durationState;

        [SerializeField]
        private WeaponUsageList weaponUsage;

        [SerializeField]
        private ActorList human;

        [SerializeField]
        private ActorWeaponList humanWeapon;

        [SerializeField]
        private ActorWeaponSkinList humanWeaponSkin;

        [SerializeField]
        private ActorArmorList humanHeadArmor;

        [SerializeField]
        private ActorArmorList humanBodyArmor;

        [SerializeField]
        private ActorArmorList humanHandArmor;

        [SerializeField]
        private ActorArmorList humanFootArmor;

        [SerializeField]
        private ActorArmorList humanOrnamentArmor;

        [SerializeField]
        private ActorUsableItemList humanRecoverItem;

        [SerializeField]
        private ActorUsableItemList humanAttackItem;

        [SerializeField]
        private ActorUsableItemList humanAuxiliaryItem;

        [SerializeField]
        private ActorNormalItemList humanNormalItem;

        [SerializeField]
        private SkillList skill;

        [SerializeField]
        private EnemyList enemy;

        [SerializeField]
        private ShopList shop;

        private void Awake()
        {
            GameInfo = gameInfo;
            Term = term;
            BattleEffect = battleEffect;
            DurationState = durationState;
            WeaponUsage = weaponUsage;
            Actor = human;
            ActorWeapon = humanWeapon;
            ActorWeaponSkin = humanWeaponSkin;
            ActorHeadArmor = humanHeadArmor;
            ActorBodyArmor = humanBodyArmor;
            ActorHandArmor = humanHandArmor;
            ActorFootArmor = humanFootArmor;
            ActorOrnamentArmor = humanOrnamentArmor;
            ActorRecoverItem = humanRecoverItem;
            ActorAttackItem = humanAttackItem;
            ActorAuxiliaryItem = humanAuxiliaryItem;
            ActorNormalItem = humanNormalItem;
            Skill = skill;
            Enemy = enemy;
            Shop = shop;
        }
    }
}
