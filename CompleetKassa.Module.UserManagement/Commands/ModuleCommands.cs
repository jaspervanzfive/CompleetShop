using Prism.Commands;

namespace CompleetKassa.Module.UserManagement.Commands
{
	public class ModuleCommands : IModuleCommands
	{
		public CompositeCommand NewCommand { get; }
		public CompositeCommand EditCommand { get; }
		public CompositeCommand DeleteCommand { get; }
		public CompositeCommand SaveCommand { get; }
		public CompositeCommand CancelCommand { get; }

		public CompositeCommand FirstNavCommand { get; }
		public CompositeCommand LastNavCommand { get; }
		public CompositeCommand PreviousNavCommand { get; }
		public CompositeCommand NextNavCommand { get; }

		public ModuleCommands ()
		{
			NewCommand = new CompositeCommand (true);
			EditCommand = new CompositeCommand (true);
			DeleteCommand = new CompositeCommand (true);
			SaveCommand = new CompositeCommand (true);
			CancelCommand = new CompositeCommand (true);

			FirstNavCommand = new CompositeCommand (true);
			LastNavCommand = new CompositeCommand (true);
			PreviousNavCommand = new CompositeCommand (true);
			NextNavCommand = new CompositeCommand (true);
		}
	}
}