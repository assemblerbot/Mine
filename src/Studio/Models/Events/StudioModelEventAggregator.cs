using EventAggregatorPlugin;

namespace Mine.Studio;

public sealed class StudioModelEventAggregator : GenericEventAggregator<IStudioModelEvent>, IStudioModelEventAggregatorReadOnly
{
}