using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    public partial class formPerfil : Form
    {
        MySqlConnection conexion = new MySqlConnection();

        public String userName, imagen;

        public formPerfil()
        {
            InitializeComponent();
        }

        private void formPerfil_Load(object sender, EventArgs e)
        {
            labelUsuario.Text = userName;
            conexion.ConnectionString = "server=remotemysql.com;Database=Pr1mdxAdrh;Uid=Pr1mdxAdrh;Pwd=fNBUrxid1O";
            cargarImagen();
        }

        public void cargarImagen()
        {
            //Cargar imagen del usuario en pictureBox
            conexion.Open();
            String CadenaSql = "select * from usuarios where nombre=?user";
            MySqlCommand comando = new MySqlCommand(CadenaSql, conexion);
            comando.Parameters.Add("?user", MySqlDbType.String).Value = userName;
            MySqlDataReader datos = comando.ExecuteReader();
            Usuario cf = new Usuario();
            while (datos.Read())
            {
                cf.Imagen = (byte[])datos["imagen"];
            }

            conexion.Close();

            MemoryStream ms = new MemoryStream(cf.Imagen);
            pictureBox1.Image = Image.FromStream(ms);
            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Volver al chat?", "Chat Dam2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                
                //Volver a formulario chat
                this.Owner.Show();
                
                Close();
            }
        }

        private void btnCambiarPass_Click(object sender, EventArgs e)
        {
            //Cambiar contraseña
            if (tbNewPass1.Text.Equals(tbNewPass2.Text))
            {
                conexion.Open();
                String cadenaSql = "update usuarios set clave=?pass where nombre=?user";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?pass", MySqlDbType.String).Value = tbNewPass1.Text;
                comando.Parameters.Add("?user", MySqlDbType.String).Value = labelUsuario.Text;
                comando.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Contraseña cambiada");
            }
            else
            {
                MessageBox.Show("Las contraseñas no coinciden");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //Borrar los mensajes del usuario
            DialogResult resultado = MessageBox.Show("¿Borrar todos los mensajes?", "Chat Dam2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                conexion.Open();
                String cadenaSql = "delete from chat where usuario=?user";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?user", MySqlDbType.String).Value = labelUsuario.Text;
                comando.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("Mensajes borrados");
            }

        }

        private void btnCambiarImg_Click(object sender, EventArgs e)
        {
            //Seleccionar imagen mediante ventana de dialogo
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imagen = openFileDialog1.FileName;
            }

            //leer y escribir datos de un fichero
            FileStream fs = new FileStream(imagen, FileMode.Open, FileAccess.Read);
            long tamanio = fs.Length;
            //métodos que simplifican la lectura de los tipos de datos primitivos de una secuencia. 
            BinaryReader br = new BinaryReader(fs);
            byte[] bloque = br.ReadBytes((int)fs.Length);
            fs.Read(bloque, 0, Convert.ToInt32(tamanio));

            conexion.Open();
            String cadenaSql = "update usuarios set imagen=?img where nombre=?user";
            MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?img", MySqlDbType.Blob).Value = bloque;
            comando.Parameters.Add("?user", MySqlDbType.String).Value = labelUsuario.Text;
            comando.ExecuteNonQuery();
            conexion.Close();

            cargarImagen();
        }
    }
}
