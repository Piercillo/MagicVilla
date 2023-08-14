﻿using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //logger despues de agregar el patch y los nugets
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {

            _logger = logger; //inicializarlo
            _db = db;
            _mapper = mapper;

        }


        [HttpGet] //siempre debes usar un verbo
        [ProducesResponseType(StatusCodes.Status200OK)]  //el endpoint que creare aqui es la lista de villas //recuerda importar
        //public ActionResult<IEnumerable<VillaDto>> GetVillas()
        public async Task<ActionResult<IEnumerable<VillaDto>>>GetVillas() //escriba el modelo y luego seleccionalo para que se agrege arriba//despues le das nombre al metodo
        {
            //PRIMER END POINT QUE ES DE TIPO http GET 
            //a trabajar con el dto
            _logger.LogInformation("Obtener las villas");
            IEnumerable<Villa> villasList = await _db.Villas.ToListAsync();
            //return Ok(_db.Villas.ToList());//seleccionando de la tabla de la base de datos
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villasList));
        }



        //otro end point que nos teronara solo una villa
        //con el id que le pasemos
        [HttpGet("id:int", Name = "GetVilla")]//hay que ponerle una ruta distinta sino da error
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)//devolver solo una villa
        {
            if (id == 0)//validacion si pasa un mal parametro
            {
                _logger.LogError("Error al traer la villa con id " + id);
                return BadRequest();//1:44:22
            }
            // villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(v=>v.Id==id);

            if (villa==null)//si encontro el registro
            {
                return NotFound();   //retorna un codigo de estado 404
            }

            return Ok(_mapper.Map<VillaDto>(villa));//le pasamos la variable villa
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //codigo de estado 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto) //el villaDto es el nombre, antes le di el modelo
        {
            if (!ModelState.IsValid) //sino esta valido retornara..
            {
                return BadRequest(ModelState);//para las siguientes lineas de codigo
            }
            if (await _db.Villas.FirstOrDefaultAsync(v=>v.Nombre.ToLower() == createDto.Nombre.ToLower()) !=null )  //validacion que no sea igual a otra que existe
            {
                ModelState.AddModelError("NombreExiste","La Villa con ese nombre ya existe!");//nombre de la validacion y mensaje que quiero mostrar
                return BadRequest(ModelState);
            }
            if (createDto==null)
            {
                return BadRequest(createDto);
            }
            //if (villaDto.Id>0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}
            //villaDto.Id = VillaStore.villaList.OrderByDescending(v=>v.Id).FirstOrDefault().Id + 1;//ordenar la lista
            //VillaStore.villaList.Add(villaDto);    //agregamos y se lo enviamos al modelo dto
            //return Ok(villaDto); no esta mal pero se puede cambiar porque en una api cuando creamosun nuevo recursoi debemos indicar la url
            Villa modelo = _mapper.Map<Villa>(createDto);
            //Villa modelo = new()
            //{//este es mi modelo
            //    //Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad,
            //};
            //agregar el registro a la base 
            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();//para que se guarde y se refleje en la base
            return CreatedAtRoute("GetVilla", new {id=modelo.Id}, modelo); //en el get le cree name es el nombre de la ruta
            
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id==0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(v=>v.Id == id);
            if (villa==null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);//no existe remove asincrono
            await _db.SaveChangesAsync();
            //VillaStore.villaList.Remove(villa);
            return NoContent();//siempre hacer un returno asi con delete
        }

        //hhtp path y put
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto) //recibido de villadto y le pondre de nombre villa dto
        {
            if (updateDto == null || id!= updateDto.Id)
            {
                return BadRequest();
            }
            //buscar el registro por id
            //var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = _mapper.Map<Villa>(updateDto);

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl   = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};
            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();//no quiero retornar el modelo
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto) //recibido de villadto y le pondre de nombre villa dto
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            //buscar el registro por id
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            //VillaUpdateDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad
            //};

            if (villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState); //verificar que el model state sea valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();//no quiero retornar el modelo
        }

    }
}
