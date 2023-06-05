﻿using AppRpgEtec.Models;
using AppRpgEtec.Services.Personagens;
using AppRpgEtec.Models.Enuns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;
        private int id;
        private string nome;
        private int pontosVida;
        private int forca;
        private int defesa;
        private int inteligencia;

        private string personagemSelecionadoId;

        public string PersonagemSelecionadoId
        { 
            set
            {
                if (value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }
       
       

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; set; }

        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService= new PersonagemService(token);
            _ = ObterClasses();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); });
            CancelarCommand = new Command(async => CancelarCadastro());
        }
        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }


        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();//Informa mundaça de estado para a View para ViewModel ou vice-versa de acordo com a herança da BaseViewModel
            }
        }
        public int PontosVida
        {
            get => pontosVida;
            set
            {
                pontosVida = value;
                OnPropertyChanged();
            }
        }

        public int Forca
        {
            get => forca;
            set
            {
                forca = value;
                OnPropertyChanged();
            }
        }

        public int Defesa
        {
            get => defesa;
            set
            {
                defesa = value;
                OnPropertyChanged();
            }
        }

        public int Inteligencia
        {
            get => inteligencia;    
            set
            {
                inteligencia = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TipoClasse> listaTiposClasse;

        public ObservableCollection<TipoClasse> ListaTiposClasse
        {
            get { return listaTiposClasse; }
            set
            {
                if(value != null)
                {
                    listaTiposClasse= value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task ObterClasses()
        {
            try
            {
                ListaTiposClasse = new ObservableCollection<TipoClasse>();
                ListaTiposClasse.Add(new TipoClasse() { Id = 1, Descricao = "Cavaleiro" });
                ListaTiposClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTiposClasse.Add(new TipoClasse() { Id = 3, Descricao = "Clerigo" });
                OnPropertyChanged(nameof(ListaTiposClasse));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private TipoClasse tipoClasseSelecionado;

        public TipoClasse TipoClasseSelecionado { 
            get { return tipoClasseSelecionado; }
            set
            {
                if(value != null)
                {
                    tipoClasseSelecionado= value;
                    OnPropertyChanged();  
                }
            }
        }



        

        public async Task SalvarPersonagem()
        {
            try
            {
                Personagem model = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.Defesa,
                    Forca = this.Forca,
                    Inteligencia = this.Inteligencia,
                    Id = this.Id,
                    Classe = (ClasseEnum)tipoClasseSelecionado.Id
                };
                if (model.Id == 0)
                    await pService.PostPersonagemAsync(model);
                else
                    await pService.PutPersonagemAsync(model);

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await Shell.Current.GoToAsync("..."); //Remove a página atual da pilha de páginas
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await pService.GetPersonagemAsync(int.Parse(personagemSelecionadoId));

                this.Nome = p.Nome;
                this.pontosVida = p.PontosVida;
                this.Defesa = p.Defesa;
                this.Forca = p.Forca;
                this.Inteligencia = p.Inteligencia;
                this.Id = p.Id;

                tipoClasseSelecionado = this.ListaTiposClasse.FirstOrDefault(TipoClasse => TipoClasse.Id == (int)p.Classe);
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

    }
}
