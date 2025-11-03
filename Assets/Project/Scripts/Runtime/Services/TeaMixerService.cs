using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Runtime.Utils;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;


namespace TeaGame.Services
{
    public class TeaMixerService : IDisposable
    {
        private CompositeDisposable _disposables = new();
        private ReactiveProperty<ItemData> _teaToCook = new();
        public IReadOnlyReactiveProperty<ItemData> TeaToCook => _teaToCook;

        public ReactiveCollection<WordOfPower> wordsForTea = new();


        [Inject]
        public void Construct()
        {

        }

        public void SetItemTea(ItemData itemData)
        {
            _teaToCook.Value = itemData;
        }



        public void Dispose()
        {
            _disposables.Dispose();
        }


        public ItemData MixTea()
        {
            if (_teaToCook == null || wordsForTea.IsEmpty())
            {
                return null;
            }

            List<TeaFlavorTag> teaFlavorTags = new(_teaToCook.Value.GetConfig<TeaConfig>().teaFlavorTags);
            List<TeaFlavorTag> teaFlavorsToRemove = new();
            foreach (var wordOfPower in wordsForTea)
            {
                foreach (var flavorInfluence in wordOfPower.wordToFlavorInfluences)
                {
                    TeaFlavorTag teaFlavorTag = flavorInfluence.teaFlavorTag;
                    switch (flavorInfluence.wordInfuence)
                    {
                        case WordInfuence.Add:
                            if (!teaFlavorTags.Contains(teaFlavorTag))
                            {
                                teaFlavorTags.Add(teaFlavorTag);
                            }
                            break;
                        case WordInfuence.Remove:
                            if (!teaFlavorsToRemove.Contains(teaFlavorTag))
                            {
                                teaFlavorsToRemove.Add(teaFlavorTag);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            for (int i = teaFlavorsToRemove.Count - 1; i >= 0; i--)
            {
                if (teaFlavorTags.Contains(teaFlavorsToRemove[i]))
                    teaFlavorTags.Remove(teaFlavorsToRemove[i]);
            }


            ItemData newTea = Copium.CreateDeepCopy(_teaToCook.Value);

            newTea.GetConfig<TeaConfig>().teaFlavorTags = teaFlavorTags;

            newTea.itemTag.Value = ItemTag.TeaReady;

            return newTea;
        }

        public float GetRating(List<TeaFlavorTag> flavors, List<TeaFlavorTag> desiredFlavors)
        {
            float rating = 0f;

            if (flavors.Count > 0)
            {
                int flavorMatches = flavors.Count(flavor => desiredFlavors.Contains(flavor));
                rating += (float)flavorMatches / (float)flavors.Count;
            }

            Debug.Log($"calculated rating: {rating}");

            return rating;
        }


    }
}