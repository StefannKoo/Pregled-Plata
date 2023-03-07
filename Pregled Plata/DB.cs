using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pregled_Plata
{
    internal class DB
    {
        private NpgsqlConnection connection;
        private NpgsqlCommand command;
        private DataTable dt;

        private List<string> bonusi;
        private List<string> odbitci;

        private string con_string = @"server=localhost; 
                                     port=5432;User Id=postgres; Password=stefan; 
                                     Database=pregled_plata_database";
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(con_string);
        }

        public void openConnection()
        {
            connection = GetConnection();

            if(connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if(connection != null)
            {
                connection.Close();
            }
        }

        public DataTable searchAllData()
        {
            command = new NpgsqlCommand("  select r.radnik_id, r.ime , r.prezime , r.preostali_dani_odmora," +
                                        "  rp.rasporedjen_od, rp.rasporedjen_do, p.naziv, nos.ime," +
                                        "  nos.prezime, pl.iznos from radnik r" +
                                        "  left join radnik_pozicija rp on r.radnik_id = rp.radnik_id left join " +
                                        "  pozicija p on rp.pozicija_id = p.pozicija_id left join nadredjena_osoba nos" +
                                        "  on r.nadredjena_osoba_id = nos.nadredjena_osoba_id left join plata pl" +
                                        "  on  pl.radnik_pozicija_id = rp.radnik_pozicija_id  where CURRENT_DATE " +
                                        "  between rp.rasporedjen_od and rp.rasporedjen_do order by r.ime", connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                dt = new DataTable();
                dt.Load(reader);
            }
            return dt;
        }

        public DataTable searchPozicije()
        {
            command = new NpgsqlCommand("select distinct naziv from pozicija ", connection);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                dt = new DataTable();
                dt.Load(reader);
            }
            return dt;
        }

        public DataTable showIstorijat()
        {
            command = new NpgsqlCommand("select r.ime, r.prezime, poz.naziv, pl.iznos, pl.datum_promjene from radnik r" +
                                       " left join radnik_pozicija rp on r.radnik_id = rp.radnik_id join pozicija poz on" +
                                       " rp.pozicija_id = poz.pozicija_id  left join plata pl on" +
                                       " rp.radnik_pozicija_id = pl.radnik_pozicija_id order by r.ime ", connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                dt = new DataTable();
                dt.Load(reader);
            }
            return dt;
        }

        public DataTable searchByDatumPozicija(string pozicija, DateTime datum)
        {
            command = new NpgsqlCommand("  select r.ime , r.prezime , r.preostali_dani_odmora," +
                                        "  rp.rasporedjen_od, rp.rasporedjen_do, p.naziv, nos.ime," +
                                        "  nos.prezime, pl.iznos from radnik r" +
                                        "  left join radnik_pozicija rp on r.radnik_id = rp.radnik_id left join " +
                                        "  pozicija p on rp.pozicija_id = p.pozicija_id left join nadredjena_osoba nos" +
                                        "  on r.nadredjena_osoba_id = nos.nadredjena_osoba_id left join plata pl" +
                                        "  on  pl.radnik_pozicija_id = rp.radnik_pozicija_id  where  rp.rasporedjen_od<=@datum" +
                                        "  and rp.rasporedjen_do>=@datum and p.naziv=@pozicija order by r.ime", connection);

            command.Parameters.AddWithValue("datum", datum);
            command.Parameters.AddWithValue("pozicija", pozicija);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                dt = new DataTable();
                dt.Load(reader);
            }
            return dt;
        }

        public int addNewPozicijaToRadnik(int radnik_id, int pozicija_id, DateTime datum_od, DateTime datum_do)
        {
            command = new NpgsqlCommand(" insert into radnik_pozicija( rasporedjen_od, rasporedjen_do, radnik_id, pozicija_id)" +
                                        "values(@datum_od,@datum_do,@radnik_id,@pozicija_id) ", connection);

            command.Parameters.AddWithValue("datum_od", datum_od);
            command.Parameters.AddWithValue("datum_do", datum_do);
            command.Parameters.AddWithValue("radnik_id", radnik_id);
            command.Parameters.AddWithValue("pozicija_id", pozicija_id);
            
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int getPozicijaId(string naziv)
        {
            command = new NpgsqlCommand("select pozicija_id from pozicija where naziv=@naziv ", connection);

            command.Parameters.AddWithValue("naziv", naziv);

            return (int)command.ExecuteScalar();
        }

        public List<string> getRadnikBonusi(int radnik_id)
        {
            command = new NpgsqlCommand("select naziv from bonusi where radnik_id=@id and aktivan=@aktivan", connection);

            command.Parameters.AddWithValue("id", radnik_id);
            command.Parameters.AddWithValue("aktivan", true);

            bonusi = new List<string>();
            using (NpgsqlDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string bonus = rdr["naziv"].ToString();
                    bonusi.Add(bonus);
                }
            }
            return bonusi;
        }

        public List<string> getRadnikOdbitci(int radnik_id)
        {
            command = new NpgsqlCommand("select naziv_odbitka from odbitci where radnik_id=@id and aktivan=@aktivan", connection);

            command.Parameters.AddWithValue("id", radnik_id);
            command.Parameters.AddWithValue("aktivan", true);

            odbitci = new List<string>();
            using (NpgsqlDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string odbitak = rdr["naziv_odbitka"].ToString();
                    odbitci.Add(odbitak);
                }
            }
            return odbitci;
        }
        public List<RadniKPozicija> getPozicijeRadnik(int radnik_id)
        {

            command = new NpgsqlCommand("select rp.radnik_pozicija_id, p.naziv from radnik_pozicija rp JOIN pozicija p on rp.pozicija_id=p.pozicija_id " +
                                        "where rp.radnik_id = @radnik_id and rp.rasporedjen_do >= now()", connection);

            command.Parameters.AddWithValue("radnik_id", radnik_id);

            List<RadniKPozicija> lista = new List<RadniKPozicija>();

            RadniKPozicija rp;

            using (NpgsqlDataReader rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    rp=new RadniKPozicija();

                    rp.naziv = rdr["naziv"].ToString();

                    rp.id = Convert.ToInt32(rdr["radnik_pozicija_id"]);

                    lista.Add(rp);

                }
            }

            return lista;
        }

        public int updateRadnikPozicija(int pozicija_id, int radnik_pozicija_id)
        {
            command = new NpgsqlCommand("UPDATE radnik_pozicija SET pozicija_id = @pozicija_id WHERE radnik_pozicija_id=@id", connection);

            command.Parameters.AddWithValue("id", radnik_pozicija_id);
            command.Parameters.AddWithValue("pozicija_id", pozicija_id);

            return Convert.ToInt32(command.ExecuteScalar());
        }

    }
     class RadniKPozicija
    {
        public int id { get; set; }

        public string naziv { get; set; }
    }
}
