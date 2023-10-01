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
    public class NumeroVillaController : ControllerBase
    {
        //logger despues de agregar el patch y los nugets
        private readonly ILogger<NumeroVillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;//variable declarada de API response

        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo,
                                                                            INumeroVillaRepositorio numeroRepo, IMapper mapper)
        {

            _logger = logger; //inicializarlo
            //_db = db;
            _villaRepo = villaRepo;
            _numeroRepo = numeroRepo;
            _mapper = mapper;
            _response = new();//inicializando mi apiresponse

        }


        [HttpGet] //siempre debes usar un verbo
        [ProducesResponseType(StatusCodes.Status200OK)]  //el endpoint que creare aqui es la lista de villas //recuerda importar
        //public ActionResult<IEnumerable<VillaDto>> GetVillas()
        public async Task<ActionResult<APIResponse>>GetNumeroVillas() //escriba el modelo y luego seleccionalo para que se agrege arriba//despues le das nombre al metodo
        {
            //PRIMER END POINT QUE ES DE TIPO http GET
            try
            {
                _logger.LogInformation("Obtener numero de villas");
                IEnumerable<NumeroVilla> numeroVillasList = await _numeroRepo.ObtenerTodos(incluirPropiedades:"Villa");
                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numeroVillasList);
                _response.statusCode = HttpStatusCode.OK;
                
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return Ok(_response);
            }
            
        }

        //otro end point que nos teronara solo una villa
        //con el id que le pasemos
        [HttpGet("{id:int}", Name = "GetNumeroVilla")]//hay que ponerle una ruta distinta sino da error
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)//devolver solo una villa
        {
            try
            {
                if (id == 0)//validacion si pasa un mal parametro
                {
                    _logger.LogError("Error al traer la numero villa con id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);//1:44:22
                }
                // villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id, incluirPropiedades:"Villa");

                if (numeroVilla == null)//si encontro el registro
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);   //retorna un codigo de estado 404
                }
                _response.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
                _response.statusCode=HttpStatusCode.OK;

                return Ok(_response);
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
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto) //el villaDto es el nombre, antes le di el modelo
        {
            try
            {
                if (!ModelState.IsValid) //sino esta valido retornara..
                {
                    return BadRequest(ModelState);//para las siguientes lineas de codigo
                }
                if (await _numeroRepo.Obtener(v => v.VillaNo == createDto.VillaNo) != null)  //validacion que no sea igual a otra que existe
                {
                    ModelState.AddModelError("ErrorMessages", "El numero de la villa ya existe!");//nombre de la validacion y mensaje que quiero mostrar
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v=>v.Id==createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "El Id de la villa no existe!");//nombre de la validacion y mensaje que quiero mostrar
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _numeroRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response); //en el get le cree name es el nombre de la ruta
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

        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);
                if (numeroVilla == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                await _numeroRepo.Remover(numeroVilla);//no existe remove asincrono

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
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.VillaNo)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (await _villaRepo.Obtener(V => V.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ErrorMessages", "El Id de la Villa No existe!");
                return BadRequest(ModelState);
            }

            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);


            await _numeroRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

        //[HttpPatch("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto) //recibido de villadto y le pondre de nombre villa dto
        //{
        //    if (patchDto == null || id == 0)
        //    {
        //        return BadRequest();
        //    }
        //    //buscar el registro por id
        //    //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        //    var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false );

        //    VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

        //    if (villa == null) return BadRequest();

        //    patchDto.ApplyTo(villaDto, ModelState); //verificar que el model state sea valido
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Villa modelo = _mapper.Map<Villa>(villaDto);

        //    await _villaRepo.Actualizar(modelo);
        //    _response.statusCode = HttpStatusCode.NoContent;
        //    //await _db.SaveChangesAsync();
        //    return Ok(_response);//no quiero retornar el modelo
        //}//3:30:57 //4:19:04

    }
}
