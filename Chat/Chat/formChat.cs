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
    public partial class formChat : Form
    {

        MySqlConnection conexion = new MySqlConnection();
        List<Usuario> ListaUsuarios = new List<Usuario>();
        List<Usuario> ListaConectados = new List<Usuario>();
        int contadorRefresco=10, contadorLogin=300, contadorChat=3;
        
        public String usuario;

        public formChat()
        {
            InitializeComponent();
        }

        private void formChat_Load(object sender, EventArgs e)
        {
            lbUser.Text = usuario;
            conexion.ConnectionString = "server=remotemysql.com;Database=Pr1mdxAdrh;Uid=Pr1mdxAdrh;Pwd=fNBUrxid1O";
            cargarChat();
            cargarListaUsuarios();
            cargarListaConectados();
            cargarImagenes(ListaUsuarios, flowLayoutPanel1);
            cargarImagenes(ListaConectados, flowLayoutPanel2);
            tbEnviar.Focus();
            timer1.Enabled = true;
        }

        private void cargarChat()
        {
            //Cargar mensajes del chat
            conexion.Open();
            String cadenaSql = "select * from chat order by 4 desc";
            MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
            MySqlDataReader datos = comando.ExecuteReader();
            
            while (datos.Read())
            {
                lbChat.Items.Add(datos["usuario"] + " : " + datos["texto"] + " : \t" + datos["fecha"]);
            }
            
            conexion.Close();
        }

        private void cargarListaUsuarios()
        {
            //Rellenar lista con todos los usuarios
            conexion.Open();
            String CadenaSql = "select * from usuarios";
            MySqlCommand comando = new MySqlCommand(CadenaSql, conexion);
            MySqlDataReader datos = comando.ExecuteReader();
            while (datos.Read())
            {

                Usuario cf = new Usuario();
                cf.Nombre = Convert.ToString(datos["nombre"]);
                cf.Clave = Convert.ToString(datos["clave"]);
                cf.Imagen = (byte[])datos["imagen"];

                ListaUsuarios.Add(cf);

            }
            
            conexion.Close();
        }

        private void cargarListaConectados()
        {
            //Rellenar lista con los usuarios conectados
            conexion.Open();
            String CadenaSql = "select * from usuarios where activo=1";
            MySqlCommand comando = new MySqlCommand(CadenaSql, conexion);
            MySqlDataReader datos = comando.ExecuteReader();
            while (datos.Read())
            {

                Usuario cf = new Usuario();
                cf.Nombre = Convert.ToString(datos["nombre"]);
                cf.Clave = Convert.ToString(datos["clave"]);
                cf.Imagen = (byte[])datos["imagen"];

                ListaConectados.Add(cf);

            }

            conexion.Close();
        }

        private void cargarImagenes(List<Usuario> userList, FlowLayoutPanel flowLayout)
        {
            //Cargar imagenes de usuarios en flowLayout
            
            //el tamaño de la lista
            //determinará el número de botones
            //que se van a contruir
            for (int i = 0; i < userList.Count; i++)
            {
                //construir el objeto tipo Button
                Button botonX = new Button();
                //propiedad para dimensionar en alto
                botonX.Height = 80;
                //propiedad ancho
                botonX.Width = 80;
                //dotar al botón de capacidad de ejecutar
                //eventos de un click
                botonX.Click += new EventHandler(mostrarInformacion);
                //fichero binario  para convertir
                //el array byte[] en un objeto Image
                MemoryStream ms = new MemoryStream(userList[i].Imagen);
                botonX.BackgroundImage = Image.FromStream(ms);
                //ajustar el tamaño de la imagen al botón
                botonX.BackgroundImageLayout = ImageLayout.Stretch;
                //asignar posición de la lista a posición
                //del botón dentro del contenedor
                //  botonX.Tag = i;
                botonX.Tag = userList[i].Nombre;
                //añadir el nuevo botón al contenedor

                flowLayout.Controls.Add(botonX);
            }
        }

        private void mostrarInformacion(object sender, EventArgs e)
        {
            //Mostrar nombre de usuario al pinchar sobre imagen
            Button botonX = (Button)sender;
            MessageBox.Show(Convert.ToString(botonX.Tag));
        }

        private void tbEnviar_KeyUp(object sender, KeyEventArgs e)
        {
            //Enviar mensaje al pulsar intro
            //Al pulsar una tecla se reinicia el contador de desconexion
            contadorLogin = 300;

            if(e.KeyCode == Keys.Enter)
            {
                conexion.Open();
                String cadenaSql = "insert into chat values(nMensaje, ?texto, ?usuario, ?fecha)";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?texto", MySqlDbType.String).Value = tbEnviar.Text;
                comando.Parameters.Add("?usuario", MySqlDbType.String).Value = lbUser.Text;
                comando.Parameters.Add("?fecha", MySqlDbType.DateTime).Value = DateTime.Now;
                comando.ExecuteNonQuery();
                conexion.Close();

                lbChat.Items.Clear();
                cargarChat();
                tbEnviar.Clear();
                tbEnviar.Focus();
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            //Boton de enviar mensaje
            conexion.Open();
            String cadenaSql = "insert into chat values(nMensaje, ?texto, ?usuario, ?fecha)";
            MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
            comando.Parameters.Add("?texto", MySqlDbType.String).Value = tbEnviar.Text;
            comando.Parameters.Add("?usuario", MySqlDbType.String).Value = lbUser.Text;
            comando.Parameters.Add("?fecha", MySqlDbType.DateTime).Value = DateTime.Now;
            comando.ExecuteNonQuery();
            conexion.Close();

            lbChat.Items.Clear();
            cargarChat();
            tbEnviar.Clear();
            tbEnviar.Focus();
            contadorLogin = 300;
        }

        private void btnPerfil_Click(object sender, EventArgs e)
        {
            
            //Esconder formulario chat
            Hide();
            //Boton para ver y modificar el perfil del usuario
            //Pasar el nombre de usuario a formulario de  perfil
            formPerfil fperfil = new formPerfil();
            fperfil.userName = usuario;
            //Abrir formulario
            //Parametro this para volver a chat desde el perfil
            fperfil.Show(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            contadorRefresco--;
            contadorLogin--;
            contadorChat--;
            //Refresco de las listas de usuarios
            if (contadorRefresco == 0)
            {
                ListaUsuarios.Clear();
                ListaConectados.Clear();
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();
                cargarListaUsuarios();
                cargarListaConectados();
                cargarImagenes(ListaUsuarios, flowLayoutPanel1);
                cargarImagenes(ListaConectados, flowLayoutPanel2);
                contadorRefresco = 10;
            }
            //Refresco del chat
            if (contadorChat == 0)
            {
                lbChat.Items.Clear();
                cargarChat();
                contadorChat = 3;
            }
            //Desconexion por inactividad
            if (contadorLogin == 0)
            {
                conexion.Open();
                String cadenaSql = "update usuarios set activo=0 where nombre=?user";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?user", MySqlDbType.String).Value = lbUser.Text;
                comando.ExecuteNonQuery();
                conexion.Close();

                //Abre el formulario propietario o padre, que es login
                this.Owner.Show();

                Close();

            }
            //Mensaje de aviso de desconexion
            if (contadorLogin < 11)
            {
                lbAviso.Text = "¡Aviso! Serás desconectado por inactividad en: " + contadorLogin;
                lbAviso.Visible = true;
            }
            else
            {
                lbAviso.Visible = false;
            }
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Estás seguro?", "Chat Dam2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                //Usuario desconectado, actualizar estado
                conexion.Open();
                String cadenaSql = "update usuarios set activo=0 where nombre=?user";
                MySqlCommand comando = new MySqlCommand(cadenaSql, conexion);
                comando.Parameters.Add("?user", MySqlDbType.String).Value = lbUser.Text;
                comando.ExecuteNonQuery();
                conexion.Close();

                //Abre el formulario propietario o padre, que es login
                this.Owner.Show();

                Close();
            }
        }


    }
}
