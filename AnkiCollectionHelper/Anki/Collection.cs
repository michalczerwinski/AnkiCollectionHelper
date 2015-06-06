using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnkiCollectionHelper.Anki.DbModel;
using AnkiCollectionHelper.Helpers;
using AnkiCollectionHelper.SqlLite;
using Newtonsoft.Json;

namespace AnkiCollectionHelper.Anki
{
    public class Collection : IDisposable
    {
        private readonly string _collectionFileName;
        private readonly SQLiteConnection _connection;
        private DeckModels _models;

        public IEnumerable<Note> Notes
        {
            get 
            {
                foreach (var note in _connection.Table<Note>())
                {
                    note.Model = Models[note.ModelId];
                    yield return note;
                }
            }
        }

        public DeckModels Models
        {
            get
            {
                if (_models == null)
                {
                    var data = _connection.Table<Decks>().Single().ModelsBlob;
                    _models = JsonConvert.DeserializeObject<DeckModels>(data);
                }
                
                return _models;
            }
        } 

        public string MediaFolder
        {
            get { return Path.ChangeExtension(_collectionFileName, "media"); }
        }

        public Collection(string collectionFileName)
        {
            _collectionFileName = collectionFileName;
            _connection = new SQLiteConnection(collectionFileName, SQLiteOpenFlags.ReadWrite);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public void UpdateNote(Note note)
        {
            note.ModifiedOnEpochSeconds = DateTime.Now.ToUniversalTime().ToEpochTime();
            note.SynchronizationCounter = -1;
            _connection.Update(note);
        }

        public void Commit()
        {
            var colRecord = _connection.Table<Decks>().Single();
            colRecord.LastModifiedOn = DateTime.Now.ToUniversalTime().ToEpochTime();
            _connection.Update(colRecord);

            _connection.Commit();            
        }
    }
}
