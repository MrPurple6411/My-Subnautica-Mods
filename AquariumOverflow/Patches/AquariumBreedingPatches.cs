using System.Collections.Generic;
using System.Linq;
using CyclopsBioReactor;
using HarmonyLib;
using RALIV.Subnautica.AquariumBreeding;
using UnityEngine;

namespace AquariumOverflow.Patches
{
    [HarmonyPatch(typeof(AquariumInfo), nameof(AquariumInfo.Get))]
    class AquariumBreedingPatches
    {
        [HarmonyPostfix]
        private static void Postfix(Aquarium aquarium, AquariumInfo __result)
        {
            //Check for valid AquariumInfo as well as make sure the aquarium is full.
            if (__result == null || aquarium.storageContainer.container.HasRoomFor(1, 1))
                return;

            SubRoot subRoot = aquarium.GetComponentInParent<SubRoot>();

            //Ensure the aquarium is build in or on a Cyclops and not a base or on nothing.
            if (subRoot == null || subRoot is BaseRoot)
                return;

            List<CyBioReactorMono> cyBioReactors = subRoot.GetComponentsInChildren<CyBioReactorMono>()?.ToList() ?? new List<CyBioReactorMono>();
            
            //Checks for any Cyclops BioReactors on the Cyclops and if none returns.
            if (cyBioReactors.Count == 0)
                return;

            double timePassed = DayNightCycle.main.timePassed;


            bool reactorsFull = false;
            //Checks all types of fish in the aquarium and collects data on how many to be put in the BioReactors
            for (int i = 0; i < __result.BreedInfo.Count; i++)
            {
                AquariumInfo.AquariumBreedTime nextBreed = __result.BreedInfo[i];

                // Check if it is actually time to breed.
                if (!reactorsFull && nextBreed.BreedTime <= timePassed)
                {
                    reactorsFull = TryOverflowToBioreactors(cyBioReactors, nextBreed.FishType, nextBreed.BreedCount);
                    nextBreed.BreedTime += 600.0;
                    AquariumInfo.Update(aquarium);
                }
            }
        }

        /// <summary>
        /// This method actually checks all the reactors for available space and inputs the fish until either all reactors are full or there are no more fish. 
        /// </summary>
        /// <param name="bioReactors"> This is the list of Cyclops BioReactors currently on the same Cyclops as the Aquarium</param>
        /// <param name="fishToBreed"> These are the fish to be distributed into the BioReactors</param>
        private static bool TryOverflowToBioreactors(List<CyBioReactorMono> bioReactors, TechType fishType, int breedCount)
        {
            Vector2int sizePerFish = CraftData.GetItemSize(fishType);
            List<CyBioReactorMono> fullReactors = new List<CyBioReactorMono>();
            for (int i = 0; i < breedCount; i++)
            {
                foreach (CyBioReactorMono reactor in bioReactors)
                {
                    if (reactor.container.HasRoomFor(sizePerFish.x, sizePerFish.y))
                    {
                        GameObject poorFishy = CraftData.InstantiateFromPrefab(fishType);
                        poorFishy.SetActive(false);
                        _ = reactor.container.AddItem(poorFishy.EnsureComponent<Pickupable>());
                        break;
                    }
                    else
                    {
                        fullReactors.Add(reactor);
                    }
                }
                fullReactors.ForEach((reactor) => bioReactors.Remove(reactor));
                fullReactors.Clear();
            }
            return breedCount > 0;
        }
    }
}
