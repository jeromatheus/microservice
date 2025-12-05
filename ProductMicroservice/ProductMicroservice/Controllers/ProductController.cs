using Microsoft.AspNetCore.Mvc;
using ProductMicroservice.Models;
using ProductMicroservice.Repository;
using System.Transactions;

namespace ProductMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase 
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound(); // Retorna 404 si no existe
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();    // Retorna 400 si objeto no vino
            }

            // Usamos TransactionScope si quisieramos atomicidad estricta, 
            // pero para este ejemplo simple llamamos al repo:
            await _productRepository.InsertProduct(product);

            // Retorna 201 Created y la URL para consultar el nuevo producto
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            // Verificar si el ID existe antes de actualizar
            var existing = await _productRepository.GetProductById(product.Id);
            if (existing == null) return NotFound();

            await _productRepository.UpdateProduct(product);

            return NoContent(); // Retorna 204 (OK sin devolver contenido)
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Verificar si existe antes de borrar para devolver 404 es
            // opcional porque en APIs idempotentes, borrar algo que no existe dar OK o NoContent
            var existing = await _productRepository.GetProductById(id);
            if (existing == null) return NotFound();

            await _productRepository.DeleteProduct(id);

            return NoContent(); // Retorna 204
        }

        // GET api/product/catalog
        [HttpGet("catalog")] 
        public async Task<IActionResult> GetCatalog()
        {
            var catalog = await _productRepository.GetCatalog();
            return Ok(catalog);
        }

        /// <summary>
        /// Crea un producto dentro de una transacción atómica arantiza la integridad de los datos
        /// </summary>
        /// <remarks>
        /// La operación consta de dos pasos: crear el producto** y registrar la auditoría.  Si el 2° paso falla, 
        /// se realiza un rollback automático, revirtiendo la creación (1° paso) para evitar inconsistencias.
        /// Nota técnica: se usa `TransactionScopeAsyncFlowOption.Enabled` para mantener la transacción activa al 
        /// usar `await` en métodos asíncronos.
        /// </remarks>
        [HttpPost("secure-create")]
        public async Task<IActionResult> CreateWithAudit([FromBody] Product product)
        {
            if (product == null) return BadRequest();

            // 1. Iniciamos el ámbito de la transacción
            // 'AsyncFlowOption.Enabled' es OBLIGATORIO cuando usas await dentro del scope.
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // PASO 1: Insertar el producto en la DB
                    await _productRepository.InsertProduct(product);

                    // PASO 2 (Simulado): Insertar un registro de auditoría o inventario
                    // Imaginemos que aquí llamamos a otro repositorio:
                    // await _auditRepository.LogAction($"Producto creado: {product.Name}");

                    // -----------------------------------------------------------
                    // ☠️ SIMULACIÓN DE ERROR (Descomenta para probar el Rollback):
                    // throw new Exception("Error simulado en el sistema de auditoría");
                    // -----------------------------------------------------------

                    // PASO 3: Confirmar la transacción
                    // Si esta línea no se ejecuta (por un error previo), TODO se deshace.
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // Log del error real (ex)
                    // No hace falta llamar a Rollback(), ocurre automático al salir del using sin Complete()
                    return StatusCode(500, "Error procesando la transacción: " + ex.Message);
                }
            }

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
    }
}