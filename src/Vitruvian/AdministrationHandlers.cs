using Archetypical.Software.Vitruvian.Common.Interfaces;
using Archetypical.Software.Vitruvian.Models.Commands;
using Archetypical.Software.Vitruvian.Models.Responses;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian
{
    public class AdministrationHandlers
    {
        private IMicrositeResolver _resolver;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new VersionConverter(), new StringEnumConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public AdministrationHandlers(IMicrositeResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task AdminDelegate(HttpContext ctx)
        {
            BaseResponse response = new UnknownResponse();
            if (ctx.Request.Method == HttpMethods.Get)
            {
                var command = new ListCommand();
                response = await ListHandlerAsync(command);
                await WriteResponse(response, ctx);
            }
            else
            {
                using (var stream = new StreamReader(ctx.Request.Body))
                {
                    var str = await stream.ReadToEndAsync();
                    var command = JsonConvert.DeserializeObject<BaseCommand>(str);
                    switch (command.Command)
                    {
                        case Models.Command.Unknown:
                            response = new UnknownResponse()
                            {
                                IsSuccessful = false,
                                Message = "Could not determine command type"
                            };
                            break;

                        case Models.Command.List:
                            response = await ListHandlerAsync(command as ListCommand);
                            break;

                        case Models.Command.Add:
                            response = await AddHandlerAsync(command as AddCommand);
                            break;

                        case Models.Command.Update:
                            break;

                        case Models.Command.Delete:
                            break;
                    }
                    await WriteResponse(response, ctx);
                }
            }
        }

        private Task WriteResponse(BaseResponse response, HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            return ctx.Response.WriteAsync(JsonConvert.SerializeObject(response, _jsonSerializerSettings));
        }

        public async Task<BaseResponse> ListHandlerAsync(ListCommand command)
        {
            var microsites = await _resolver.GetAllAsync();
            return await Task.FromResult(new ListResponse()
            {
                Microsites = microsites
            });
        }

        public async Task<BaseResponse> AddHandlerAsync(AddCommand command)
        {
            var result = await _resolver.RegisterAsync(command.Microsite);
            var addResponse = new AddResponse();
            addResponse.Microsite = result.microsite;
            addResponse.IsSuccessful = result.success;
            addResponse.Message = result.message;
            return addResponse;
        }
    }
}