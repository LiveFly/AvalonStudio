using AvalonStudio.Extensibility;
using AvalonStudio.Extensibility.Dialogs;
using AvalonStudio.Projects;
using AvalonStudio.Shell;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvalonStudio.Extensibility.Templating;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Standard.SolutionExplorer
{
    class NewItemDialogViewModel : ModalDialogViewModelBase
    {
        private IProjectFolder folder;

        private TemplateViewModel selectedTemplate;

        private ObservableCollection<TemplateViewModel> templates;

        public NewItemDialogViewModel(IProjectFolder folder) : base("New Item")
        {
            var shell = IoC.Get<IShell>();
            var templateManager = IoC.Get<TemplateManager>();            

            templates = new ObservableCollection<TemplateViewModel>(templateManager.ListItemTemplates("").Select(t=>new TemplateViewModel(t)).ToList());

            SelectedTemplate = templates.FirstOrDefault();

            this.folder = folder;

            OKCommand = ReactiveCommand.Create(async () =>
            {
                await templateManager.CreateTemplate(
                    SelectedTemplate.Template, 
                    folder.LocationDirectory, 
                    SelectedTemplate.Parameters
                    .Where(p=>!string.IsNullOrEmpty(p.Value.Trim()))
                    .Select(p=>(p.Name.ToLower(), p.Value))
                    .ToArray());                

                Close();
            });

            UpdateTemplatesCommand = ReactiveCommand.Create(() =>
            {
                Task.Run(() =>
                {
                    templateManager.UpdateDefaultTemplates();
                });
            });
        }

        public TemplateViewModel SelectedTemplate
        {
            get { return selectedTemplate; }
            set { this.RaiseAndSetIfChanged(ref selectedTemplate, value); }
        }

        public ObservableCollection<TemplateViewModel> Templates
        {
            get { return templates; }
            set { this.RaiseAndSetIfChanged(ref templates, value); }
        }

        public IProjectFolder Folder
        {
            get { return folder; }
            set { this.RaiseAndSetIfChanged(ref folder, value); }
        }

        public override ReactiveCommand OKCommand { get; protected set; }

        public ReactiveCommand UpdateTemplatesCommand { get; }
    }
}