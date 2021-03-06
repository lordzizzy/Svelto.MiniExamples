using System.Collections;
using Svelto.ECS.Components;
using Svelto.ECS.EntityStructs;
using Svelto.Tasks.ExtraLean;

namespace Svelto.ECS.MiniExamples.Example1
{
    public class LookingForFoodDoofusesEngine : IQueryingEntitiesEngine
    {
        public LookingForFoodDoofusesEngine(IEntityFunctions entityFunctions) { _entityFunctions = entityFunctions; }
        
        public void Ready() { SearchFoodOrGetHungry().RunOn(DoofusesStandardSchedulers.doofusesLogicScheduler); }

        IEnumerator SearchFoodOrGetHungry()
        {
            while (true)
            {
                var doofuses =
                    entitiesDB.QueryEntities<PositionEntityStruct, VelocityEntityStruct, HungerEntityStruct>(
                        GameGroups.DOOFUSES, out var count);
                
                var doofusesVelocity = doofuses.Item2;                
                var foods =
                    entitiesDB.QueryEntities<PositionEntityStruct, MealEntityStruct>(GameGroups.FOOD, out var foodcount);
                var foodPositions = foods.Item1;
                var mealStructs = foods.Item2;
                var hungerStructs = doofuses.Item3;

                for (int doofusIndex = 0; doofusIndex < count; doofusIndex++)
                {
                    float currentMin = float.MaxValue;
                    
                    doofusesVelocity[doofusIndex].velocity = new ECSVector3();

                    ECSVector3 closerComputeDirection = default;

                    for (int foodIndex = 0; foodIndex < foodcount; foodIndex++)
                    {
                        var computeDirection = foodPositions[foodIndex].position;
                        computeDirection.Sub(doofuses.Item1[doofusIndex].position);
                        var sqrModule = computeDirection.SqrMagnitude();

                        if (currentMin > sqrModule)
                        {
                            currentMin = sqrModule;
                            closerComputeDirection = computeDirection;

                            //food found
                            if (sqrModule < 2)
                            {   //close enough to eat
                                hungerStructs[doofusIndex].hunger-=2;
                                mealStructs[foodIndex].eaters++;
                                
                                closerComputeDirection = default;
                                
                                break; //close enough let's save some computations
                            }
                            //going toward food, not breaking as closer food can spawn
                        }
                    }
                    
                    doofusesVelocity[doofusIndex].velocity.x = closerComputeDirection.x;
                    doofusesVelocity[doofusIndex].velocity.z = closerComputeDirection.z;
                    
                    hungerStructs[doofusIndex].hunger++;
                }

                yield return null;
            }
        }

        public IEntitiesDB entitiesDB { private get; set; }

        readonly IEntityFunctions _entityFunctions;
    }
}