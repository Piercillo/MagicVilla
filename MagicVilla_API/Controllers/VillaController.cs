using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //logger despues de agregar el patch y los nugets
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;//variable declarada de API response

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {

            _logger = logger; //inicializarlo
            //_db = db;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();//inicializando mi apiresponse

        }


        [HttpGet] //siempre debes usar un verbo
        [ProducesResponseType(StatusCodes.Status200OK)]  //el endpoint que creare aqui es la lista de villas //recuerda importar
        //public ActionResult<IEnumerable<VillaDto>> GetVillas()
        public async Task<ActionResult<APIResponse>>GetVillas() //escriba el modelo y luego seleccionalo para que se agrege arriba//despues le das nombre al metodo
        {
            //PRIMER END POINT QUE ES DE TIPO http GET
            try
            {
                _logger.LogInformation("Obtener las villas");
                IEnumerable<Villa> villasList = await _villaRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villasList);
                _response.statusCode = HttpStatusCode.OK;
                
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                
            }
            return _response;

        }

        //otro end point que nos teronara solo una villa
        //con el id que le pasemos
        [HttpGet("{id:int}", Name = "GetVilla")]//hay que ponerle una ruta distinta sino da error
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)//devolver solo una villa
        {
            try
            {
                if (id == 0)//validacion si pasa un mal parametro
                {
                    _logger.LogError("Error al traer la villa con id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);//1:44:22
                }
                // villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)//si encontro el registro
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);   //retorna un codigo de estado 404
                }
                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response.statusCode=HttpStatusCode.OK;

                return Ok(_response);//le pasamos la variable villa
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //codigo de estado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto) //el villaDto es el nombre, antes le di el modelo
        {
            try
            {
                if (!ModelState.IsValid) //sino esta valido retornara..
                {
                    return BadRequest(ModelState);//para las siguientes lineas de codigo
                }
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)  //validacion que no sea igual a otra que existe
                {
                    ModelState.AddModelError("ErrorMessages", "La Villa con ese nombre ya existe!");//nombre de la validacion y mensaje que quiero mostrar
                    return BadRequest(ModelState);
                }
                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                Villa modelo = _mapper.Map<Villa>(createDto);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response); //en el get le cree name es el nombre de la ruta
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
            
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _villaRepo.Obtener(v => v.Id == id);
                if (villa == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                await _villaRepo.Remover(villa);//no existe remove asincrono

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);//siempre hacer un returno asi con delete
            }
            catch (Exception ex)
            {
                _response.IsExitoso=false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
            
        }

        //hhtp path y put
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            Villa modelo = _mapper.Map<Villa>(updateDto);


            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked: false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);


            if (villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);


            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

    }
}
