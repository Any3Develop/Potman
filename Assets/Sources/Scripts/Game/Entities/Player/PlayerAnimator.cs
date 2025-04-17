using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using R3;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IRuntimeEntity))]
    [DisallowMultipleComponent]
    public class PlayerAnimator : MonoBehaviour, IPlayerAnimator, IEntityComponent
    {
        private enum AnimatorLayer
        {
            Default = 0,
            Legs = 1,
            Hands = 2,
            Wrists = 3
        }
        
        private static readonly int VelocityX = Animator.StringToHash(nameof(VelocityX));
        private static readonly int VelocityY = Animator.StringToHash(nameof(VelocityY));
        private static readonly int AttackSpeed = Animator.StringToHash(nameof(AttackSpeed));
        private static readonly int ReloadSpeed = Animator.StringToHash(nameof(ReloadSpeed)); // TODO does it need ?
        private static readonly int MoveSpeed = Animator.StringToHash(nameof(MoveSpeed));
        private static readonly int HandsType = Animator.StringToHash(nameof(HandsType));
        private static readonly int ActionType = Animator.StringToHash(nameof(ActionType));
        private static readonly int ActionTrigger = Animator.StringToHash(nameof(ActionTrigger));
        private static readonly int RecoilTrigger = Animator.StringToHash(nameof(RecoilTrigger));
        private static readonly int ReloadTrigger = Animator.StringToHash(nameof(ReloadTrigger));
        
        private readonly List<AnimatorClipInfo> clipsInfo = new();
        private IRuntimeEntity entity;
        private Animator animator;
        private IDisposable subscribes;
        private IDisposable actionClip;

        public bool Enabled => !IsDisposed && enabled && animator && animator.enabled;
        public bool IsDisposed { get; private set; }
        
        public void Init()
        {
            if (entity != null)
                return;
            
            entity = GetComponent<IPlayerEntity>();
            animator = entity.Mapper.RecursiveMap<Animator>();
            Enable(false);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            StopCurrentAction();
            IsDisposed = true;
            subscribes?.Dispose();
            clipsInfo.Clear();
            subscribes = null;
            actionClip = null;
            animator = null;
            entity = null;
        }
        
        private void OnDestroy() => Dispose();
        
        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = animator.enabled = value;
            StopCurrentAction();
            subscribes?.Dispose();
            subscribes = null;
            
            if (Enabled)
            {
                var stats = entity.StatsCollection;
                var builder = new DisposableBuilder();
                // TODO uncomment this when all stats will be correct (1 second equal 1 unit of stat)
                // builder.Add(stats.SubscribeInitStat(StatType.AttackSpeed, stat => animator.SetFloat(AttackSpeed, stat.CurrentFloat)));
                // builder.Add(stats.SubscribeInitStat(StatType.MoveSpeed, stat => animator.SetFloat(MoveSpeed, stat.CurrentFloat)));

                subscribes = builder.Build();
                builder.Dispose();
                return;
            }
            
            if (animator)
                animator.StopPlayback();
        }

        public void SetHands(int type)
        {
            if (!Enabled)
                return;
            
            animator.SetFloat(HandsType, type);
        }

        public void Recoil()
        {
            if (!Enabled)
                return;
            
            StopCurrentAction();
            animator.SetTrigger(RecoilTrigger);
        }

        public void Reload(float time)
        {
            if (!Enabled)
                return;
            
            StopCurrentAction();
            animator.SetFloat(ReloadSpeed, time);
            animator.SetTrigger(ReloadTrigger);
        }
        
        public void Action(int type)
        {
            if (!Enabled)
                return;

            StopCurrentAction();
            animator.SetFloat(ActionType, type);
            animator.SetTrigger(ActionTrigger);
            animator.GetCurrentAnimatorClipInfo((int) AnimatorLayer.Default, clipsInfo);
            if (clipsInfo.Count == 0)
            {
                Debug.LogError($"[{nameof(PlayerAnimator)}.{nameof(Action)}] No animation clip info for action type : {type}.");
                return;
            }
            
            animator.SetLayerWeight((int)AnimatorLayer.Legs, 0f);
            animator.SetLayerWeight((int)AnimatorLayer.Hands, 0f);
            var clip = clipsInfo.First().clip;
            var playTime = TimeSpan.FromSeconds(clip.length);
            var task = clip.isLooping
                ? Observable.Interval(playTime)
                : Observable.Timer(playTime);
            
            actionClip = task
                .Do(onDispose: StopCurrentAction,
                    onCompleted: _ => StopCurrentAction(),
                    onErrorResume: _ => StopCurrentAction())
                .Subscribe();
        }

        public void Move(Vector3 dir)
        {
            if (!Enabled)
                return;

            StopCurrentAction();
            animator.SetFloat(VelocityX, dir.x);
            animator.SetFloat(VelocityY, dir.z);
        }

        private void StopCurrentAction()
        {
            if (actionClip == null)
                return;

            var memo = actionClip; // prevent deadlock
            actionClip = null;
            memo?.Dispose();
            clipsInfo.Clear();
            
            if (!Enabled)
                return;
            
            animator.SetLayerWeight((int) AnimatorLayer.Legs, 1f);
            animator.SetLayerWeight((int) AnimatorLayer.Hands, 1f);
        }
    }
}