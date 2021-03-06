using System.Collections;
using Boxtopia.GUIs.Generic;
using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.ExtraLean;
using UnityEngine;

namespace Boxtopia.GUIs
{
    public class GenericGUIInteraction : IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { get; set; }
        
        public GenericGUIInteraction(IEntityStreamConsumerFactory generateConsumer)
        {
            _generateConsumer = generateConsumer;
        }
        
        public void Ready()
        {
            PollForButtonClicked().RunOn(ExtraLean.BoxtopiaSchedulers.UIScheduler);
        }
        
        IEnumerator PollForButtonClicked()
        {
            using (var consumer =
                _generateConsumer.GenerateConsumer<ButtonEntityStruct>("StandardButtonActions", 1))
            {
                while (true)
                {
                    while (consumer.TryDequeue(out var entity))
                    {
                        var entitiesDb = entitiesDB;
                        if (entity.message == ButtonEvents.WANNAQUIT)
                        {
                            yield return Yield.It;
                        }
                        
                        if (entity.message == ButtonEvents.QUIT)
                        {
                            Svelto.Console.Log("Quitting now");

                            Application.Quit();

                            yield break;
                        }
                        
                        if (entity.message == ButtonEvents.OK || entity.message == ButtonEvents.CANCEL)
                        {// The buttons are contextual to the GUI that owns them, so the group must be the same
                            var entityHierarchy =
                                entitiesDb.QueryEntity<EntityHierarchyStruct>(entity.ID);
                            
                            var guiEntityViewStructs =
                                entitiesDb.QueryEntities<GUIEntityViewStruct>(entityHierarchy.parentGroup, out var count);

                            for (int i = 0; i < count; i++)
                                guiEntityViewStructs[i].guiRoot.enabled = false;
                        }
                    }

                    yield return null;
                }
            }
        }

        readonly IEntityStreamConsumerFactory _generateConsumer;
    }
}