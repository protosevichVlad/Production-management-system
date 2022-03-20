using System;
using System.Collections.Generic;
using System.Linq;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.Components
{
    public class TreeViewNodeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Tuple<int, string>> Leaves { get; set; }
        public List<TreeViewNodeViewModel> Nodes { get; set; }

        public TreeViewNodeViewModel() {}
        public TreeViewNodeViewModel(Directory directory, bool showLeaves)
        {
            Id = directory.Id;
            Title = directory.DirectoryName;
            Nodes = directory.Childs.Select(x => new TreeViewNodeViewModel(x, showLeaves)).ToList();
            if (showLeaves)
                Leaves = directory?.Tables?.Select(x => new Tuple<int, string>(x.Id, x.DisplayName))?.ToList() 
                         ?? new List<Tuple<int, string>>();
            else
                Leaves = new List<Tuple<int, string>>();
            
        }
    }
    public class TreeViewViewModel
    {
        public bool ShowPath { get; set; }
        public int SelectedId { get; set; }
        public TreeViewNodeViewModel Tree { get; set; }

        public TreeViewViewModel() {}
        public TreeViewViewModel(Directory directory, bool showLeaves)
        {
            Tree = new TreeViewNodeViewModel(directory, showLeaves);
        }
    }
}