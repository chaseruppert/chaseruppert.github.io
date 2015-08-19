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

" Solarized:
" git clone git://github.com/altercation/vim-colors-solarized.git ~/.vim/bundle/vim-colors-solarized

" matchit:
" git clone https://github.com/edsono/vim-matchit.git ~/.vim/bundle/vim-matchit

" BufOnly (close all other buffers but current)
" git clone https://github.com/vim-scripts/BufOnly.vim.git ~/.vim/bundle/vim-BufOnly

" Bclose
" https://github.com/rbgrouleff/bclose.vim

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
"set nowrap
set nojoinspaces
set pastetoggle=<F12>           " pastetoggle (sane indentation on pastes)

":au BufReadPost *.build set syntax=html
au BufReadPost *.html set syntax=html

" vim macros
source $HOME/.vim/bundle/vim-matchit/plugin/matchit.vim  
source $HOME/.vim/bundle/vim-BufOnly/plugin/BufOnly.vim  

nnoremap <Tab> :bnext<CR>
nnoremap <S-Tab> :bprevious<CR>

" toggle word wrap mode
nnoremap <silent> <F2> :set wrap!<CR>

" copy full filename to clipboard (Windows)
nnoremap <silent> <F3> :let @* = expand("%:p")<CR>

" Airline config
let g:airline#extensions#tabline#enabled = 1

" CtrlP settings
"let g:ctrlp_match_window = 'bottom,order:ttb,results:100'
let g:ctrlp_match_window = 'bottom,order:ttb'
let g:ctrlp_switch_buffer = 0

" Maybe need to set these if you have problems with CtrlP finding files
let g:ctrlp_max_files=0
let g:ctrlp_working_path_mode = ""
let g:ctrlp_max_depth=50
let g:ctrlp_custom_ignore = {
    \ 'file': '\v(\.c|\.cpp|\.h|\.hh|\.cxx|\.py|\.html|\.js|\.css|\.less|\.xml|\.xsd|\.build)@<!$'
    \ }

" The Silver Searcher
" On windows: install using Chocolatey:
" https://github.com/ggreer/the_silver_searcher/wiki/Windows
" https://chocolatey.org/
" @powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin
" choco install ag
if executable('ag')
  " Use ag over grep
  set grepprg=ag\ --nogroup\ --nocolor\ --ignore\ *.map\ --ignore\ *.log\ --ignore\ *.opensdf

  " Use ag in CtrlP for listing files. Lightning fast and respects .gitignore
  let g:ctrlp_user_command = 'ag -l --nocolor -g "" %s'

  " ag is fast enough that CtrlP doesn't need to cache,
  " but it still seems like it might be helpful??
  let g:ctrlp_use_caching = 1
endif

" bind K to grep word under cursor
"nnoremap K :grep! "\b<C-R><C-W>\b"<CR>:cw<CR>
nnoremap K :lgrep! "\b<C-R><C-W>\b"<CR>:lw<CR>
