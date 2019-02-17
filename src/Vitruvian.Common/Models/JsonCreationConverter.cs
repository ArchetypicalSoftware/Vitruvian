using Archetypical.Software.Vitruvian.Common.Models.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian.Common.Models
{
    public static class SerializerSettings
    {
        public static JsonSerializerSettings CommandJsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new VersionConverter(), new StringEnumConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        protected bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }

    public class BaseCommandConverter : JsonCreationConverter<BaseCommand>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private const string CommandField = "Command";

        protected override BaseCommand Create(Type objectType, JObject jObject)
        {
            if (FieldExists(CommandField, jObject))
            {
                Command command = Command.Unknown;
                var enumValue = jObject.Value<string>(CommandField);
                if (int.TryParse(enumValue, out int enumInt))
                {
                    command = (Command)enumInt;
                }
                else
                {
                    Enum.TryParse<Command>(enumValue, out command);
                }

                switch (command)
                {
                    case Command.List:
                        return new ListCommand();

                    case Command.Add:
                        return new AddCommand();

                    case Command.Update:
                        return new UpdateCommand();

                    case Command.Delete:
                        return new DeleteCommand();

                    default:
                        return new UnknownCommand();
                }
            }

            return new UnknownCommand();
        }
    }
}