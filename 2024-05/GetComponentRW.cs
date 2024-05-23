// Get a rewritable component from an entity. When writing to this component, it changes the entity data without writing back via a set component call
var xPositionLists = SystemAPI.GetComponentRW<XPositionLists>(entity);

// Use this to see if an entity has an optional component
var lookup = SystemAPI.GetComponentLookup<XPositionLists>();
var rw = lookup.GetRefRWOptional(entity);
if(rw.IsValid){
  rw.ValueRW.TweenFrameSeekIndex++;
}