using System.Collections.Generic;
using System.Linq;
using System;
// prompt: listeye 100 sentetik data ekle
// other done manually
namespace App.Models
{
    public static class ClassInformationTable
    {
        // Static list that serves as our in-memory "database"
        private static List<ClassInformationModel> _classDatabase = new List<ClassInformationModel>();

        // Counter for auto-increment Id assignment
        private static int _nextId = 1;

        // Flag to ensure synthetic data is generated only once.
        private static bool _syntheticDataGenerated = false;

        public static void AddClass(ClassInformationModel newClass)
        {
            newClass.Id = _nextId++;
            _classDatabase.Add(newClass);
        }

        public static void EditClass(int id, ClassInformationModel updatedClass)
        {
            var existingClass = _classDatabase.FirstOrDefault(c => c.Id == id);
            if (existingClass != null)
            {
                existingClass.ClassName = updatedClass.ClassName;
                existingClass.StudentCount = updatedClass.StudentCount;
                existingClass.Description = updatedClass.Description;
            }
        }

        public static void DeleteClass(int id)
        {
            var classToDelete = _classDatabase.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                _classDatabase.Remove(classToDelete);
            }
        }

      
        private static void EnsureSyntheticData()
        {
            if (!_syntheticDataGenerated)
            {
                int currentCount = _classDatabase.Count;
                for (int i = currentCount + 1; i <= 100; i++)
                {
                    var synthetic = new ClassInformationModel
                    {
                        ClassName = $"Sample Class {i}",
                        StudentCount = (i % 30) + 1, 
                        Description = $"Description for class {i}"
                    };

                    AddClass(synthetic);
                }
                _syntheticDataGenerated = true;
            }
        }

        public static List<ClassInformationModel> GetAllClasses()
        {
            
            EnsureSyntheticData();
            
            return new List<ClassInformationModel>(_classDatabase);
        }

        public static ClassInformationModel GetClassById(int id)
        {
            return _classDatabase.FirstOrDefault(c => c.Id == id);
        }
    }
}
