using System;
using System.Collections.Generic;
using Potman.Common.UIService;
using UnityEngine;

namespace Potman.Game.UI.GameStatistics
{
    public class StatisticsWindow : UISafeWindowBase
    {
        [SerializeField] private StatisticView prototype;
        private readonly Dictionary<string, StatisticView> statisticViews = new();

        protected override void OnInit()
        {
            base.OnInit();
            prototype.SetActive(false);
        }

        public override void Hidden()
        {
            base.Hidden();
            RemoveAll();
        }

        public void Remove(string id)
        {
            AssertId(id);
            if (!statisticViews.TryGetValue(id, out var view))
                return;

            statisticViews.Remove(id);
            Destroy(view.gameObject);
        }

        public void RemoveAll()
        {
            foreach (var view in statisticViews.Values) 
                Destroy(view.gameObject);

            statisticViews.Clear();
        }

        public void AddSatistic(string id, string value)
        {
            AssertId(id);

            prototype.SetActive(true);
            if (!statisticViews.TryGetValue(id, out var view))
                statisticViews.Add(id, view = Instantiate(prototype, Content));
            
            prototype.SetActive(false);
            UpdateStatistic(view, value);
        }

        public void UpdateStatistic(string id, string value)
        {
            AssertId(id);

            if (statisticViews.TryGetValue(id, out var view))
                UpdateStatistic(view, value);
        }

        private void UpdateStatistic(StatisticView view, string value)
        {
            view.Label.SetText(value);
        }

        private void AssertId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Statistic id is null or empty.");
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            RemoveAll();
        }
    }
}