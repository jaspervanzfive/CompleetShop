using Prism.Commands;

namespace CompleetKassa.Module.Product.Commands
{
    class IModuleCommands
    {
        CompositeCommand NewCommand { get; }
        CompositeCommand EditCommand { get; }
        CompositeCommand DeleteCommand { get; }
        CompositeCommand SaveCommand { get; }
        CompositeCommand CancelCommand { get; }

        CompositeCommand FirstNavCommand { get; }
        CompositeCommand LastNavCommand { get; }
        CompositeCommand PreviousNavCommand { get; }
        CompositeCommand NextNavCommand { get; }
    }
}
