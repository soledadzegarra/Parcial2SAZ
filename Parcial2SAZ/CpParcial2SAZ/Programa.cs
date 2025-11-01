using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClnParcial2Lfms;
using ClnParcial2SAZ;

namespace CpParcial2SAZ
{
    public partial class FrmPrograma : Form
    {
        private bool esNuevo = false;

        public FrmPrograma()
        {
            InitializeComponent();
        }
        private void listar()
        {
            var lista = ProgramaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible = false;
            dgvLista.Columns["estado"].Visible = false;
            dgvLista.Columns["titulo"].HeaderText = "Título";
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["duracion"].HeaderText = "Duración (minutos)";
            dgvLista.Columns["productor"].HeaderText = "Productor";
            dgvLista.Columns["fechaEstreno"].HeaderText = "Fecha de Estreno";
            dgvLista.Columns["canal"].HeaderText = "Canal";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario de Registro";

            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["titulo"];
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void cargarCanal()
        {
            var listaCanales = CanalCln.listar();
            cbxCanal.DataSource = listaCanales;
            cbxCanal.DisplayMember = "nombre";
            cbxCanal.ValueMember = "id";
            cbxCanal.SelectedIndex = -1;
        }

        private void Programa_Load(object sender, EventArgs e)
        {
            cargarCanal();
            Size = new Size(651, 374);
            listar();

            // Empezar con el área de datos deshabilitada hasta que el usuario cree/edite
            gbxDatos.Enabled = false;
            pnlAcciones.Enabled = true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;

            // Habilitar los campos para edición/alta
            gbxDatos.Enabled = true;
            limpiar();
            txtTitulo.Focus();

            // Desactivar botones principales mientras se edita/crea
            pnlAcciones.Enabled = false;

            Size = new Size(651, 536);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;

            esNuevo = false;

            // Habilitar los campos para edición
            gbxDatos.Enabled = true;

            // Desactivar botones principales mientras se edita
            pnlAcciones.Enabled = false;

            Size = new Size(651, 536);

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var programa = ProgramaCln.obtenerUno(id);
            txtTitulo.Text = programa.titulo;
            txtDescripcion.Text = programa.descripcion;
            nudDuracion.Text = programa.duracion.HasValue ? programa.duracion.Value.ToString() : "";
            txtProductor.Text = programa.productor;
            dtpFechaEstreno.Value = programa.fechaEstreno.HasValue ? programa.fechaEstreno.Value : DateTime.Today;
            cbxCanal.SelectedValue = programa.idCanal;
            txtTitulo.Focus();
        }
        private void limpiar()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            nudDuracion.Value = 0;
            txtProductor.Clear();
            dtpFechaEstreno.Value = DateTime.Today;
            cbxCanal.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Size = new Size(651, 374);

            // Volver a estado no edición: deshabilitar inputs y habilitar acciones principales
            gbxDatos.Enabled = false;

            limpiar();

            pnlAcciones.Enabled = true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validar()) return;

            // Asegurar SelectedValue no es null antes de castear
            if (cbxCanal.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un canal.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var programa = new CadParcial2SAZ.Programa
            {
                titulo = txtTitulo.Text.Trim(),
                descripcion = txtDescripcion.Text.Trim(),
                duracion = (int)nudDuracion.Value,
                productor = txtProductor.Text.Trim(),
                fechaEstreno = dtpFechaEstreno.Value,
                idCanal = Convert.ToInt32(cbxCanal.SelectedValue),
                usuarioRegistro = "admin",
                fechaRegistro = DateTime.Now,
                estado = 1
            };

            if (esNuevo)
            {
                ProgramaCln.insertar(programa);
                MessageBox.Show("Programa registrado correctamente.", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                programa.id = id;
                ProgramaCln.actualizar(programa);
                MessageBox.Show("Programa actualizado correctamente.", "Actualización exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            listar();
            Size = new Size(651, 374);

            // Termina modo edición: habilitar acciones principales y deshabilitar inputs
            pnlAcciones.Enabled = true;
            gbxDatos.Enabled = false;

            limpiar();

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var resultado = MessageBox.Show("¿Está seguro de eliminar el programa seleccionado?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                ProgramaCln.eliminar(id, "admin");
                listar();
                MessageBox.Show("Programa eliminado correctamente.", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmPrograma_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) listar();
        }
        private bool validar()
        {
            bool esValido = true;
            erpTitulo.Clear();
            erpDescripcion.Clear();
            erpDuracion.Clear();
            erpProductor.Clear();
            erpCanal.Clear();
            erpfechaEstreno.Clear();

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                erpTitulo.SetError(txtTitulo, "El título es obligatorio.");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria.");
                esValido = false;
            }
            if (nudDuracion.Value <= 0)
            {
                erpDuracion.SetError(nudDuracion, "La duración debe ser mayor a cero.");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtProductor.Text))
            {
                erpProductor.SetError(txtProductor, "El productor es obligatorio.");
                esValido = false;
            }
            if (cbxCanal.SelectedIndex == -1)
            {
                erpCanal.SetError(cbxCanal, "Debe seleccionar un canal.");
                esValido = false;
            }
            if (dtpFechaEstreno.Value > DateTime.Today)
            {
                erpfechaEstreno.SetError(dtpFechaEstreno, "La fecha de estreno no puede ser futura.");
                esValido = false;
            }
            return esValido;
        }
    }
}