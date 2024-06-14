using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Animations;

namespace Map
{
    /// <summary>
    /// 提供API以访问玩家队伍成员。
    /// </summary>
    public class PlayerParty : MonoBehaviour
    {
        private static PlayerParty Instance { get; set; }
        public static GameObject Player { get; private set; }
        public static Mover PlayerMover { get; private set; }
        public static Vector3 PlayerPosition => Player.transform.position;
        public static List<GameObject> Followers { get; private set; }
        public static List<Mover> FollowerMovers { get; private set; }
        public static Static.CharacterSkin BlankSkin => Instance.blankSkin;

        public static void RefreshPlayerSkin()
        {
            var members = Party.GetBattleActorList();
            Player.GetComponent<Mover>().Skin = members[0].CharacterSkin;
            var colliderType = Player.GetComponent<RectangleCollider>().Type;
            for (int i = 0; i < Followers.Count; i++)
            {
                if (members.Count > i + 1)
                {
                    Followers[i].GetComponent<Mover>().Skin = members[i + 1].CharacterSkin;
                    Followers[i].GetComponent<RectangleCollider>().Type = colliderType;
                }
                else
                {
                    Followers[i].GetComponent<Mover>().Skin = BlankSkin;
                    Followers[i].GetComponent<RectangleCollider>().Type = RectangleCollider
                        .ColliderType
                        .None;
                }
            }
        }

        public static void SetPartyPosition(Vector3 newPosition)
        {
            var mover = Player.GetComponent<Mover>();
            mover.Position = newPosition;

            foreach (var follower in Followers)
            {
                var fm = follower.GetComponent<Mover>();
                fm.Position = newPosition;
                fm.Direction = mover.Direction;
            }
        }

        public static void SetPartyPosition(Vector3 newPosition, Mover.DirectionType newDirection)
        {
            var mover = Player.GetComponent<Mover>();
            mover.Position = newPosition;
            mover.Direction = newDirection;

            foreach (var follower in Followers)
            {
                var fm = follower.GetComponent<Mover>();
                fm.Position = newPosition;
                fm.Direction = newDirection;
            }
        }

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private GameObject followerPrefab;

        [SerializeField]
        private Static.CharacterSkin blankSkin;

        [SerializeField]
        private PositionConstraint positionConstraint;

        private void Awake()
        {
            Instance = this;

            //生成玩家对象并设置位置
            Player = Instantiate(playerPrefab, transform);
            PlayerMover = Player.GetComponent<Mover>();
            PlayerMover.Position = ResourceManager.GameInfo.startPosition;

            //生成跟随玩家的队友并设置位置
            Followers = new();
            FollowerMovers = new();
            for (int i = 0; i < Party.MaxBattleMembers; i++)
            {
                var f = Instantiate(followerPrefab, transform);
                Followers.Add(f);
                var fm = f.GetComponent<Mover>();
                fm.Position = ResourceManager.GameInfo.startPosition;
                FollowerMovers.Add(fm);
            }

            //建立跟随关系
            Player.GetComponent<Mover>().Follower = FollowerMovers[0];
            FollowerMovers[0].IsFollow = true;
            for (int i = 0; i < FollowerMovers.Count - 1; i++)
            {
                FollowerMovers[i].Follower = FollowerMovers[i + 1];
                FollowerMovers[i + 1].IsFollow = true;
            }

            //绑定摄像机，使摄像机跟随玩家移动
            positionConstraint.AddSource(
                new() { sourceTransform = PlayerMover.Renderer.transform, weight = 1 }
            );
        }

        private void Start()
        {
            RefreshPlayerSkin();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
