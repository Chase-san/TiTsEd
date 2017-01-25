﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TiTsEd.Common;
using TiTsEd.Model;
using TiTsEd.View;

namespace TiTsEd.ViewModel {
    public delegate void SaveRequiredChanged(object sender, BoolEventArgs e);

    public sealed class VM : BindableBase {
        public event SaveRequiredChanged SaveRequiredChanged;
        public event EventHandler FileOpened;

        const string AppTitle = "TiTsEd";

        readonly List<string> _externalPaths = new List<string>();
        AmfFile _currentFile;

        private VM() {
        }

        public static void Create() {
            Instance = new VM();
        }

        public static VM Instance { get; private set; }

        public XmlDataSet Data { get; private set; }

        public GameVM Game { get; private set; }

        public bool SaveRequired { get; private set; }

        public string FileVersion {
            get { return _currentFile == null ? "" : _currentFile.GetString("version"); }
        }

        public bool HasData {
            get { return _currentFile != null; }
        }

        public void Load(string path, SerializationFormat expectedFormat, bool createBackup) {
            FileManager.TryRegisterExternalFile(path);
            var file = new AmfFile(path);
            var dataVersion = file.GetString("version");

            if (file.Error != null) {
                var box = new ExceptionBox();
                box.Title = "Could not read file.";
                box.Message = "TiTsEd could not read this file correctly. Maybe it was corrupted or generated by an old version of Flash. Continuing may make TiTsEd unstable or cause it to corrupt the file. It is strongly advised that you cancel this operation.";
                box.Path = file.FilePath;
                box.ExceptionMessage = file.Error.Mesg;
                box.IsWarning = true;
                var result = box.ShowDialog(ExceptionBoxButtons.Continue, ExceptionBoxButtons.Cancel);

                Logger.Error(file.Error.Mesg);
                if (result != ExceptionBoxResult.Continue) return;
            } else if (String.IsNullOrEmpty(dataVersion)) {
                var box = new ExceptionBox();
                box.Title = "File version too old.";
                box.Message = "TiTsEd may not be able to read this file correctly as it was generated by an older version of TiTs. Continuing may make TiTsEd unstable or cause it to corrupt the file. It is strongly advised that you cancel this operation.";
                box.IsWarning = true;
                var result = box.ShowDialog(ExceptionBoxButtons.Continue, ExceptionBoxButtons.Cancel);

                Logger.Error(String.Format("{0} TiTs data version: {1}.", box.Title, dataVersion));
                if (result != ExceptionBoxResult.Continue) return;
            }

            // I would like to test dataVersion here some day to ensure that it's not too old of a version,
            // however, as long as Fen keeps occasionally pushing crappy version strings (e.g. 0.8.4.8d),
            // that can't happen.  Ideally, I'd like to see them switch to a segmented version system.

            // Sanity checks: see if the save can be re-saved as-is OR if any of the top-level property names are invalid as identifiers.
            if (!file.CanBeSaved(SerializationFormat.Slot) || HasBadPropertyNames(file)) {
                var box = new ExceptionBox();
                box.Title = "File could not be read correctly.";
                box.Message = "TiTsEd could not read this file correctly, it is likely corrupted. Cancelling this operation.";
                box.IsWarning = true;
                box.ShowDialog(ExceptionBoxButtons.OK);

                Logger.Error(String.Format("{0} TiTs data version: {1}.", box.Title, dataVersion));
                return;
            }

            // Sanity check: ensure the actual format matches the expected format (just a warning to the user about mixing up the formats).
            if (file.Format != expectedFormat) {
                var box = new ExceptionBox();
                box.Title = "File format different from expected format.";
                box.Message = "The file is actually a " + (file.Format == SerializationFormat.Slot ? "\"Slot\"" : "\"Save to File\"") + " format save, but it was loaded as though it were a " + (expectedFormat == SerializationFormat.Slot ? "\"Slot\"" : "\"Save to File\"") + " format save.\n\nThe two formats are not compatible, so care should be taken to ensure you do not confuse the two, since TiTs can only load each format in a specific way.  Attempting to load a \"Slot\" save as though it were a \"Save to File\" save, or vice versa, will cause TiTs to think the save is corrupt, at best, or see the save deleted, at worse. Do not mix them up.";
                box.IsWarning = true;
                box.ShowDialog(ExceptionBoxButtons.OK);
            }

            if (createBackup) FileManager.CreateBackup(path);
            _currentFile = file;

            XmlData.Select(XmlData.Files.TiTs);
            Data = XmlData.Current;
            Game = new GameVM(_currentFile, Game);

            OnPropertyChanged("Data");
            OnPropertyChanged("Game");
            OnPropertyChanged("HasData");
            UpdateAppTitle();
            VM.Instance.NotifySaveRequiredChanged(false);
            if (FileOpened != null) FileOpened(null, null);
        }

        // More file corruption testing
        private bool HasBadPropertyNames(AmfFile file) {
            var identiferRe = new Regex(@"^[$A-Za-z_][$A-Za-z0-9_]*$");
            foreach (var pair in file) {
                if (pair.Key.GetType() != typeof(string)) return true;
                else if (!identiferRe.IsMatch((string)pair.Key)) return true;
            }
            return false;
        }

        public void Save(string path, SerializationFormat format) {
            bool error = false;
            try {
                Game.BeforeSerialization();
                _currentFile.Save(path, format);
                FileManager.TryRegisterExternalFile(path);
            } catch (SecurityException) {
                error = true;
            } catch (UnauthorizedAccessException) {
                error = true;
            }

            if (error) {
                var box = new ExceptionBox();
                box.Title = "Permissions problem";
                box.Message = "TiTsEd does not have permission to write over this file or its backup.";
                box.Path = path;
                box.IsWarning = true;
                box.ShowDialog(ExceptionBoxButtons.Cancel);
            } else {
                VM.Instance.NotifySaveRequiredChanged(false);
            }
        }

        public void UpdateAppTitle() {
            string title = HasData ? Game.Name : "<unknown>";
            if (SaveRequired) title += "\u202F*";
            title += "  |  " + AppTitle;

            if (Application.Current.MainWindow != null)       // May be null on autoload.
                Application.Current.MainWindow.Title = title; // Databinding does not work for this
        }

        public void NotifySaveRequiredChanged(bool saveRequired = true) {
            if (saveRequired == SaveRequired) {
                UpdateAppTitle();
                return;
            }

            SaveRequired = saveRequired;
            UpdateAppTitle();
            if (SaveRequiredChanged != null) SaveRequiredChanged(null, new BoolEventArgs(saveRequired));
        }
    }


    public interface IArrayVM : IUpdatableList {
        void Create();
        void Delete(int index);
        void MoveItemToIndex(int sourceIndex, int destIndex);
    }

    public class GeneralObjectVM : ObjectVM {
        public GeneralObjectVM(AmfObject obj)
            : base(obj) {

        }
    }

    public abstract class ObjectVM : BindableBase {
        protected Dictionary<AmfObject, List<FlagItem>> flagData;

        protected List<FlagItem> getFlagList(AmfObject obj, XmlEnum[] data) {
            if (flagData == null) {
                flagData = new Dictionary<AmfObject, List<FlagItem>>();
            }
            List<FlagItem> flags = null;
            if (flagData.ContainsKey(obj)) {
                flags = flagData[obj];
            } else {
                flags = new List<FlagItem>();
                foreach (XmlEnum e in data) {
                    flags.Add(new FlagItem(obj, e));
                }
                flagData.Add(obj, flags);
            }
            return flags;
        }

        protected readonly AmfObject _obj;

        public AmfObject GetAmfObject() {
            return _obj;
        }

        protected ObjectVM(AmfObject obj) {
            _obj = obj;
        }

        public bool HasValue(object key) {
            return _obj.Contains(key);
        }

        public object GetValue(object key) {
            return _obj[key];
        }

        public double GetDouble(object key) {
            return _obj.GetDouble(key);
        }

        public int GetInt(object key, int? defaultValue = null) {
            return _obj.GetInt(key, defaultValue);
        }

        public string GetString(object key) {
            return _obj.GetString(key);
        }

        public bool GetBool(object key) {
            return _obj.GetBool(key);
        }

        public AmfObject GetObj(object key) {
            return _obj.GetObj(key);
        }

        public virtual bool SetValue(object key, object value, [CallerMemberName] string propertyName = null) {
            return SetValue(_obj, key, value, propertyName);
        }

        protected bool SetDouble(object key, double value, [CallerMemberName] string propertyName = null) {
            return SetValue(key, value, propertyName);
        }
    }

    public abstract class ArrayVM<TResult> : UpdatableCollection<AmfObject, TResult>, IArrayVM {
        readonly AmfObject _object;

        protected ArrayVM(AmfObject obj, Func<AmfObject, TResult> selector)
            : base(obj.Select(x => x.Value as AmfObject), selector) {
            _object = obj;
        }

        protected ArrayVM(AmfObject obj, IEnumerable<AmfObject> values, Func<AmfObject, TResult> selector)
            : base(values, selector) {
            _object = obj;
        }

        public void Create() {
            AmfObject node = CreateNewObject();
            Add(node);
        }

        protected TResult Add(AmfObject node) {
            _object.Push(node);
            Update();
            VM.Instance.NotifySaveRequiredChanged(true);
            return this[_object.DenseCount - 1];
        }

        public void Delete(int index) {
            _object.Pop(index);
            Update();
            VM.Instance.NotifySaveRequiredChanged(true);
        }

        public void MoveItemToIndex(int sourceIndex, int destIndex) {
            if (sourceIndex == destIndex) return;
            _object.Move(sourceIndex, destIndex);
            Update();
            VM.Instance.NotifySaveRequiredChanged(true);
        }

        void IArrayVM.MoveItemToIndex(int sourceIndex, int destIndex) {
            MoveItemToIndex(sourceIndex, destIndex);
        }

        protected abstract AmfObject CreateNewObject();
    }

    public class FlagItem {
        readonly AmfObject _object;
        readonly XmlEnum _value;
        public FlagItem(AmfObject obj, XmlEnum value) {
            _object = obj;
            _value = value;
        }

        public String ItemName {
            get { return _value.Name; }
        }

        //Here is the real magic
        public bool ItemChecked {
            get {
                //check all indexes of _object for this value if so return true
                int i = 0;
                int id = 0;
                while ((id = _object.GetInt(i++, -1234)) != -1234) {
                    if (id == _value.ID) {
                        return true;
                    }
                }
                return false;
            }
            set {
                if (value) {
                    //check all indexes of _object for this value
                    int i = -1;
                    int id = 0;
                    while ((id = _object.GetInt(++i, -1234)) != -1234) {
                        if (id == _value.ID) {
                            //we already exist
                            return;
                        }
                    }
                    //add this flag to the object
                    _object.Push(_value.ID);
                } else {
                    int i = -1;
                    int id = 0;
                    while ((id = _object.GetInt(++i, -1234)) != -1234) {
                        if (id == _value.ID) {
                            _object.Pop(i);
                            return;
                        }
                    }
                }
            }
        }
    }
}
