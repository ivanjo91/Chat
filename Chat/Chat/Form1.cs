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

    public partial class formLogin : Form
    {

        String imagen;

        MySqlConnection conexion = new MySqlConnection();

        public formLogin()
        {
            InitializeComponent();
        }

        private void formLogin_Load(object sender, EventArgs e)
        {
            conexion.ConnectionString = "server=remotemysql.com;Database=Pr1mdxAdrh;Uid=Pr1mdxAdrh;Pwd=fNBUrxid1O";
            tbUser.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (comprobarUsuario())
            {
                estadoUsuario(true);
                limpiar();
                //Ocultar formulario login
                Hide();
                //Abrir formulario chat
                formChat fchat = new formChat();
                fchat.usuario = tbUser.Text;
                //Pasar parametro this para poder volver a formulario login al cerrar formulario chat
                fchat.Show(this);
            }
            else
            {
                MessageBox.Show("Usuario no registrador o contraseña incorreta");
                limpiar();
            }

        }

        public Boolean comprobarUsuario()
        {
            //Comprobar usuario y contraseña correctos   
            conexion.Open();
            String cadenaSql = "select * from usuarios where nombre=?user and clave=?pwd";
            MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?user", MySqlDbType.String).Value = tbUser.Text;
            comando.Parameters.Add("?pwd", MySqlDbType.String).Value = tbPass.Text;
            MySqlDataReader datos = comando.ExecuteReader();
            int contador = 0;

            while (datos.Read())
            {
                contador++;
            }

            conexion.Close();

            if (contador == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
                                    
        }

        public void estadoUsuario(Boolean estado)
        {
            //Modificar estado del usuario en BD
            conexion.Open();
            String cadenaSql="";
            if (estado)
            {
                //Usuario conectado
                cadenaSql = "update usuarios set activo=1 where nombre=?user";
            }
            else
            {
                //Usuario desconectado
                cadenaSql = "update usuarios set activo=0 where nombre=?user";
            }
            MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?user", MySqlDbType.String).Value = tbUser.Text;
            comando.ExecuteNonQuery();
            conexion.Close();
        }

        public void limpiar()
        {
            //Limpiar campo contraseña
            tbPass.Text = "";
            tbUser.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Estás seguro?", "Chat Dam2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                estadoUsuario(false);
                Close();
            }
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            //Boton de registrar usuario
            try
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
                String cadenaSql = "insert into usuarios values(?nombre, ?clave, ?img, 0)";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?nombre", MySqlDbType.String).Value = tbUser.Text;
                comando.Parameters.Add("?clave", MySqlDbType.String).Value = tbUser.Text;
                comando.Parameters.Add("?img", MySqlDbType.Blob).Value = bloque;
                comando.ExecuteNonQuery();
                conexion.Close();

            }catch(MySqlException ex)
            {
                MessageBox.Show("Error clave duplicada: " + ex.Message);
            }
            

        }

        private void tbPass_KeyUp(object sender, KeyEventArgs e)
        {
            //Hacer login al pulsar intro en textbox de contraseña

            if(e.KeyCode== Keys.Enter)
            {
                if (comprobarUsuario())
                {
                    estadoUsuario(true);
                    limpiar();
                    //Ocultar formulario login
                    Hide();
                    //Abrir formulario chat
                    formChat fchat = new formChat();
                    fchat.usuario = tbUser.Text;
                    //Pasar parametro this para poder volver a formulario login al cerrar formulario chat
                    fchat.Show(this);
                    
                    
                }
                else
                {
                    MessageBox.Show("Usuario no registrador o contraseña incorreta");
                    limpiar();
                }
            }
            
        }
    }
}
