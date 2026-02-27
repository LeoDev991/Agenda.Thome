import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import api from '../services/api';

function Appointments() {
  const [appointments, setAppointments] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form, setForm] = useState({
    patientName: '',
    patientEmail: '',
    patientPhone: '',
    scheduledAt: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadAppointments();
  }, []);

  const loadAppointments = async () => {
    try {
      const response = await api.get('/appointments');
      setAppointments(response.data);
    } catch (err) {
      console.error('Erro ao carregar consultas', err);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const payload = {
        ...form,
        scheduledAt: new Date(form.scheduledAt).toISOString(),
      };

      if (editing) {
        await api.put(`/appointments/${editing}`, payload);
      } else {
        await api.post('/appointments', payload);
      }

      setShowForm(false);
      setEditing(null);
      resetForm();
      loadAppointments();
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao salvar consulta.');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (appointment) => {
    const date = new Date(appointment.scheduledAt);
    const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16);

    setForm({
      patientName: appointment.patientName,
      patientEmail: appointment.patientEmail,
      patientPhone: appointment.patientPhone,
      scheduledAt: local,
    });
    setEditing(appointment.id);
    setShowForm(true);
    setError('');
  };

  const handleDelete = async (id) => {
    if (!confirm('Deseja realmente excluir esta consulta?')) return;

    try {
      await api.delete(`/appointments/${id}`);
      loadAppointments();
    } catch (err) {
      alert(err.response?.data?.message || 'Erro ao excluir.');
    }
  };

  const resetForm = () => {
    setForm({ patientName: '', patientEmail: '', patientPhone: '', scheduledAt: '' });
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const bookingLink = `${window.location.origin}/booking/${user?.bookingToken}`;

  return (
    <div>
      <div className="header">
        <h2>Agenda.Thome</h2>
        <div>
          <span style={{ marginRight: 16 }}>Olá, {user?.name}</span>
          <button onClick={handleLogout}>Sair</button>
        </div>
      </div>

      <div className="container">
        <div className="booking-token">
          <strong>Seu link de agendamento:</strong>
          {bookingLink}
        </div>

        <div className="top-bar">
          <h1>Consultas</h1>
          <button
            className="btn btn-small"
            onClick={() => {
              setShowForm(true);
              setEditing(null);
              resetForm();
              setError('');
            }}
          >
            + Nova Consulta
          </button>
        </div>

        {appointments.length === 0 && (
          <p className="empty">Nenhuma consulta agendada.</p>
        )}

        {appointments.map((apt) => (
          <div className="card" key={apt.id}>
            <h3>{apt.patientName}</h3>
            <p>📧 {apt.patientEmail}</p>
            <p>📱 {apt.patientPhone}</p>
            <p>📅 {formatDate(apt.scheduledAt)}</p>
            <div className="card-actions">
              <button
                className="btn btn-small btn-secondary"
                onClick={() => handleEdit(apt)}
              >
                Editar
              </button>
              <button
                className="btn btn-small btn-danger"
                onClick={() => handleDelete(apt.id)}
              >
                Excluir
              </button>
            </div>
          </div>
        ))}
      </div>

      {showForm && (
        <div className="modal-overlay" onClick={() => setShowForm(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h2>{editing ? 'Editar Consulta' : 'Nova Consulta'}</h2>

            {error && <div className="error">{error}</div>}

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Nome do Paciente</label>
                <input
                  type="text"
                  value={form.patientName}
                  onChange={(e) => setForm({ ...form, patientName: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>E-mail do Paciente</label>
                <input
                  type="email"
                  value={form.patientEmail}
                  onChange={(e) => setForm({ ...form, patientEmail: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Telefone</label>
                <input
                  type="text"
                  value={form.patientPhone}
                  onChange={(e) => setForm({ ...form, patientPhone: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Data e Horário</label>
                <input
                  type="datetime-local"
                  value={form.scheduledAt}
                  onChange={(e) => setForm({ ...form, scheduledAt: e.target.value })}
                  required
                />
              </div>

              <div className="modal-actions">
                <button className="btn" type="submit" disabled={loading}>
                  {loading ? 'Salvando...' : 'Salvar'}
                </button>
                <button
                  className="btn btn-secondary"
                  type="button"
                  onClick={() => setShowForm(false)}
                >
                  Cancelar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Appointments;
