using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface ICitaObserver
    {
        void OnCitaConfirmada(Cita cita);
    }
}
