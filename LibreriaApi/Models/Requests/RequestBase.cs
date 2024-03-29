﻿namespace LibreriaApi.Models.Requests
{
    public abstract class RequestBase
    {
        protected const string
            REQUIRED_ERROR_MESSAGE = "Es necesario indicar el {0}.",
            MAX_LENGTH_ERROR_MESSAGE = "El {0} excede el tamaño permitido({1}).",
            FORMAT_ERROR_MESSAGE = "El {0} no tiene el formato correcto.",
            RANGE_ERROR_MESSAGE = "El {0} está fuera de los valores permitidos({1},{2}).";
    }
}
