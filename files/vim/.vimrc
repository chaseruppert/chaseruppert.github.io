" Sources:
" http://dougblack.io/words/a-good-vimrc.html
" https://github.com/spf13/spf13-vim/blob/3.0/.vimrc

" To install Pathogen (cygwin):
"mkdir -p /cygdrive/c/Users/username/.vim/autoload /cygdrive/c/Users/username/.vim/bundle && \
"curl -LSso /cygdrive/c/Users/username/.vim/autoload/pathogen.vim https://tpo.pe/pathogen.vim

" Airline:
" git clone https://github.com/bling/vim-airline /cygdrive/c/Users/username/.vim/bundle/vim-airline
"
" CtrlP:
" git clone https://github.com/kien/ctrlp.vim.git /cygdrive/c/Users/username/.vim/bundle/ctrlp.vim

" Setup your environment first
if has('win32') || has('win64')
    set runtimepath=$HOME/.vim,$VIM/vimfiles,$VIMRUNTIME,$VIM/vimfiles/after,path/to/home.vim/after
endif

execute pathogen#infect()
syntax enable

set background=dark
colorscheme solarized

scriptencoding utf-8

set cindent
set smartindent
set autoindent
set tabstop=4 shiftwidth=4 softtabstop=4 expandtab

filetype plugin indent on
set number
set ruler                   " Show the ruler
set rulerformat=%30(%=\:b%n%y%m%r%w\ %l,%c%V\ %P%) " A ruler on steroids
set showcmd                 " Show partial commands in status line and selected characters/lines in visual mode

set lines=35 columns=150

set wildmode=longest,list,full
set wildmenu

set showmatch
set incsearch           " search as characters are entered
set hlsearch            " highlight matches

set foldenable
set foldlevelstart=10
set foldnestmax=10
" space open/closes folds
nnoremap <space> za
set foldmethod=indent

set mouse=a                 " Automatically enable mouse usage
set mousehide               " Hide the mouse cursor while typing
set viewoptions=folds,options,cursor,unix,slash " Better Unix / Windows compatibility
set virtualedit=onemore             " Allow for cursor beyond last character
set history=1000                    " Store a ton of history (default is 20)
set spell                           " Spell checking on
set hidden                          " Allow buffer switching without saving
set tabpagemax=15               " Only show 15 tabs
set showmode                    " Display the current mode
set cursorline                  " Highlight current line
set backspace=indent,eol,start
set smartcase  " Case sensitive when uc present
set nowrap
set nojoinspaces
set pastetoggle=<F12>           " pastetoggle (sane indentation on pastes)

":au BufReadPost *.build set syntax=html
au BufReadPost *.html set syntax=html
source $HOME/.vim/macros/matchit.vim 

nnoremap <Tab> :bnext<CR>
nnoremap <S-Tab> :bprevious<CR>

" Airline config
let g:airline#extensions#tabline#enabled = 1

" CtrlP settings
let g:ctrlp_match_window = 'bottom,order:ttb'
let g:ctrlp_switch_buffer = 0
let g:ctrlp_working_path_mode = 0
"let g:ctrlp_user_command = 'ag %s -l --nocolor -g ""'
