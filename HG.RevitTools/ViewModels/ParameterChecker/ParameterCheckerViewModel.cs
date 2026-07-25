using Autodesk.Revit.DB;
using HG.RevitTools.Models.ParameterChecker;
using HG.RevitTools.Services.ParameterChecker;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HG.RevitTools.ViewModels.ParameterChecker
{
    public class ParameterCheckerViewModel : INotifyPropertyChanged
    {
        private readonly Document _document;

        private readonly IReadOnlyCollection<Element>
            _lightingFixtures;

        private readonly ParameterValueService
            _parameterValueService;

        private ParameterOption _selectedParameter;

        private ParameterValueOption _selectedValue;

        public string HeaderText { get; }

        public List<ParameterOption> Parameters { get; }

        public List<ParameterValueOption> ParameterValues
        {
            get;
            private set;
        }

        public ParameterOption SelectedParameter
        {
            get
            {
                return _selectedParameter;
            }

            set
            {
                if (_selectedParameter == value)
                {
                    return;
                }

                _selectedParameter = value;

                OnPropertyChanged();

                LoadParameterValues();
            }
        }

        public ParameterValueOption SelectedValue
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                if (_selectedValue == value)
                {
                    return;
                }

                _selectedValue = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanCheckElements));
            }
        }

        public bool CanCheckElements
        {
            get
            {
                return SelectedParameter != null &&
                       SelectedValue != null;
            }
        }

        public ParameterCheckerViewModel(
            Document document,
            IReadOnlyCollection<Element> lightingFixtures,
            IEnumerable<ParameterOption> parameters,
            ParameterValueService parameterValueService,
            string headerText)
        {
            _document = document;
            _lightingFixtures = lightingFixtures;
            _parameterValueService = parameterValueService;

            HeaderText = headerText;

            Parameters = parameters?
                .OrderBy(parameter => parameter.Name)
                .ToList()
                ?? new List<ParameterOption>();

            ParameterValues =
                new List<ParameterValueOption>();

            if (Parameters.Count > 0)
            {
                SelectedParameter = Parameters.First();
            }
        }

        private void LoadParameterValues()
        {
            ParameterValues =
                new List<ParameterValueOption>();

            SelectedValue = null;

            if (SelectedParameter != null)
            {
                ParameterValues =
                    _parameterValueService.GetUniqueValues(
                        _document,
                        _lightingFixtures,
                        SelectedParameter);
            }

            OnPropertyChanged(nameof(ParameterValues));

            if (ParameterValues.Count > 0)
            {
                SelectedValue = ParameterValues.First();
            }

            OnPropertyChanged(nameof(CanCheckElements));
        }

        public event PropertyChangedEventHandler
            PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}