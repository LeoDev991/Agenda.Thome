import { useState } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api';

function Booking() {
  const { token } = useParams();
  const [date, setDate] = useState('');
  const [slots, setSlots] = useState([]);
  const [selectedSlot, setSelectedSlot] = useState(null);
  const [form, setForm] = useState({
    patientName: '',
    patientEmail: '',
    patientPhone: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [loadingSlots, setLoadingSlots] = useState(false);

  const loadSlots = async () => {
    if (!date) return;
    setError('');
    setLoadingSlots(true);
    setSelectedSlot(null);

    try {
      const response = await api.get(`/booking/${token}/slots`, {
        params: { date },
      });
      setSlots(response.data);
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao buscar horários.');
      setSlots([]);
    } finally {
      setLoadingSlots(false);
    }
  };

  const handleBook = async (e) => {
    e.preventDefault();
    if (!selectedSlot) {
      setError('Selecione um horário.');
      return;
    }

    setError('');
    setLoading(true);

    try {
      await api.post(`/booking/${token}/book`, {
        ...form,
        scheduledAt: selectedSlot,
      });

      setSuccess('Consulta agendada com sucesso!');
      setForm({ patientName: '', patientEmail: '', patientPhone: '' });
      setSelectedSlot(null);
      loadSlots();
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao agendar.');
    } finally {
      setLoading(false);
    }
  };

  const formatHour = (dateStr) => {
    const d = new Date(dateStr);
    return d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  };

  return (
    <div className="page" style={{ maxWidth: 500 }}>
      <h1>Agendar Consulta</h1>

      {error && <div className="error">{error}</div>}
      {success && <div className="success">{success}</div>}

      <div className="form-group">
        <label>Escolha uma data</label>
        <div style={{ display: 'flex', gap: 8 }}>
          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
            style={{ flex: 1 }}
          />
          <button
            className="btn btn-small"
            onClick={loadSlots}
            disabled={!date || loadingSlots}
            type="button"
          >
            {loadingSlots ? 'Buscando...' : 'Buscar'}
          </button>
        </div>
      </div>

      {slots.length > 0 && (
        <>
          <label style={{ fontSize: '0.9rem', fontWeight: 500, color: '#475569' }}>
            Horários disponíveis:
          </label>
          <div className="slots-grid">
            {slots.map((slot) => (
              <div
                key={slot.dateTime}
                className={`slot ${
                  !slot.isAvailable
                    ? 'unavailable'
                    : selectedSlot === slot.dateTime
                    ? 'selected'
                    : 'available'
                }`}
                onClick={() => slot.isAvailable && setSelectedSlot(slot.dateTime)}
              >
                {formatHour(slot.dateTime)}
              </div>
            ))}
          </div>

          {selectedSlot && (
            <form onSubmit={handleBook} style={{ marginTop: 24 }}>
              <div className="form-group">
                <label>Seu nome</label>
                <input
                  type="text"
                  value={form.patientName}
                  onChange={(e) => setForm({ ...form, patientName: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Seu e-mail</label>
                <input
                  type="email"
                  value={form.patientEmail}
                  onChange={(e) => setForm({ ...form, patientEmail: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Seu telefone</label>
                <input
                  type="text"
                  value={form.patientPhone}
                  onChange={(e) => setForm({ ...form, patientPhone: e.target.value })}
                  required
                />
              </div>

              <button className="btn" type="submit" disabled={loading}>
                {loading ? 'Agendando...' : 'Confirmar Agendamento'}
              </button>
            </form>
          )}
        </>
      )}
    </div>
  );
}

export default Booking;
