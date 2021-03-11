using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AirportParking.Models;
using Newtonsoft.Json;

namespace AirportParking
{
    public class DataHelper : IDataHelper
    {
        private const string AIRCRAFT_TYPE_PATH = "data/aircraft-types.json";
        private const string PARKING_SPACE_PATH = "data/parking-spaces.json";
        
        public IEnumerable<AircraftType> GetAircraftTypes()
        {
            return LoadEntities<AircraftType>(AIRCRAFT_TYPE_PATH);
        }
        
        public AircraftType GetAircraftType(string id)
        {
            return GetAircraftTypes().FirstOrDefault(type => type.Id == id);
        }
        
        public void AddOrUpdateAircraftType(AircraftType type)
        {
            AddOrUpdateEntity(AIRCRAFT_TYPE_PATH, type, s => s.Id == type.Id);
        }

        public void RemoveAircraftType(string id)
        {
            RemoveEntity<AircraftType>(AIRCRAFT_TYPE_PATH, s => s.Id == id);
        }
        
        public IEnumerable<ParkingSpace> GetParkingSpaces()
        {
            return LoadEntities<ParkingSpace>(PARKING_SPACE_PATH);
        }
        
        public ParkingSpace GetParkingSpace(int id)
        {
            return GetParkingSpaces().FirstOrDefault(space => space.Id == id);
        }

        public void AddOrUpdateParkingSpace(ParkingSpace space)
        {
            AddOrUpdateEntity(PARKING_SPACE_PATH, space, s => s.Id == space.Id);
        }

        public void RemoveParkingSpace(int id)
        {
            RemoveEntity<ParkingSpace>(PARKING_SPACE_PATH, s => s.Id == id);
        }

        private IEnumerable<T> LoadEntities<T>(string sourceFilePath)
        {
            return JsonConvert.DeserializeObject<IEnumerable<T>>(File.ReadAllText(sourceFilePath));
        }
        
        private void SaveEntities<T>(string sourceFilePath, IEnumerable<T> entities)
        {
            File.WriteAllText(sourceFilePath, JsonConvert.SerializeObject(entities));
        }

        private void AddOrUpdateEntity<T>(string sourceFilePath, T entity, Predicate<T> selector)
        {
            var entities = LoadEntities<T>(sourceFilePath).ToList();
            var index = entities.FindIndex(selector);
            if (index >= 0)
            {
                entities.RemoveAt(index);
                entities.Insert(index, entity);
            }
            else
            {
                entities.Add(entity);
            }

            SaveEntities(sourceFilePath, entities);
        }

        private void RemoveEntity<T>(string sourceFilePath, Func<T, bool> selector)
        {
            var entities = LoadEntities<T>(sourceFilePath).ToList();
            var match = entities.FirstOrDefault(selector);
            if (match != null)
            {
                entities.Remove(match);
            }
            else
            {
                throw new Exception("Could not remove entry, item not found");
            }
            
            SaveEntities(sourceFilePath, entities);
        }
    }
}