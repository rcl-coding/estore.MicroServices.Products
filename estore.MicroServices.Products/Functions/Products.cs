#nullable disable
using estore.MicroServices.Products.DataContext;
using estore.MicroServices.Products.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace estore.MicroServices.Products.Functions
{
    public class Products 
    {
        private readonly ProductDbContext _context;

        public Products(ProductDbContext context)
        {
            _context = context;
        }

        [Function("Product_GetAll_V1")]
        public async Task<HttpResponseData> RunGetAllV1([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/product/all")] HttpRequestData req)
        {
            try
            {
                List<Product> _products = await _context.Products.ToListAsync();

                if (_products?.Count > 0)
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json ; charset=utf-8");
                    response.WriteString(JsonSerializer.Serialize(_products));
                    return response;

                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString(ex.Message);
                return response;
            }
        }

        [Function("Product_GetById_V1")]
        public async Task<HttpResponseData> RunGetByIdV1([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/product/id/{id:int}")] HttpRequestData req, int id)
        {
            try
            {
                Product _product = await _context.Products.FindAsync(id);

                if (_product != null)
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json ; charset=utf-8");
                    response.WriteString(JsonSerializer.Serialize(_product));
                    return response;

                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString(ex.Message);
                return response;
            }
        }

        [Function("Product_CreateOrUpdate_V1")]
        public async Task<HttpResponseData> RunCreateOrUpdateV1([HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/product")] HttpRequestData req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (!string.IsNullOrEmpty(requestBody))
                {
                    Product _product = JsonSerializer.Deserialize<Product>(requestBody);

                    if(_product.Id < 1)
                    {
                        _context.Products.Add(_product);
                        await _context.SaveChangesAsync();

                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "application/json ; charset=utf-8");
                        response.WriteString(JsonSerializer.Serialize(_product));
                        return response;
                    }
                    else
                    {
                        Product existingProduct = await _context.Products
                           .Where(w => w.Id == _product.Id)
                           .AsNoTracking()
                           .FirstOrDefaultAsync();

                        if (existingProduct != null)
                        {
                            _context.Attach(_product).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            var response = req.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Content-Type", "application/json ; charset=utf-8");
                            response.WriteString(JsonSerializer.Serialize(_product));
                            return response;
                        }
                        else
                        {
                            var response = req.CreateResponse(HttpStatusCode.NotFound);
                            return response;
                        }
                    }
                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                    response.WriteString("The product entity was not received in the body of the request");
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString(ex.Message);
                return response;
            }
        }

        [Function("Product_Delete_V1")]
        public async Task<HttpResponseData> RunDeleteV1([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "v1/product/id/{id:int}")] HttpRequestData req, int id)
        {
            try
            {
                Product _product = await _context.Products.FindAsync(id);

                if (_product != null)
                {
                    _context.Products.Remove(_product);
                    await _context.SaveChangesAsync();

                    var response = req.CreateResponse(HttpStatusCode.OK);
                    return response;

                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "text/plain ; charset=utf-8");
                response.WriteString(ex.Message);
                return response;
            }
        }
    }
}
