﻿using System.Linq.Expressions;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class //interfaz de repositorio 
    {//voy a agregar todos los modelos aqui//en esa T recibimos cualquier tipo de entidad
        Task Crear(T entidad);

        Task<List<T>>ObtenerTodos(Expression<Func<T, bool>>? filtro = null);

        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked=true);

        Task Remover(T entidad);

        Task Grabar();

    }
}