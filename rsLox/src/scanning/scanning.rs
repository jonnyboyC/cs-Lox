use scanning::token::Token;
use scanning::token_type::TokenType;

pub struct Scanner {
  pub source: String,
  pub tokens: Vec<Token>,
  pub start: u64,
  pub current: u64,
  pub line: u64,
}

fn (keyword: &str) -> Option<TokenType> {
  match keyword {
      "and" => Some(TokenType::And)
      "break" => Some(TokenType::Break),
      "class" => Some(TokenType::Class),
      "else" => Some(TokenType::Else),
      "false" => Some(TokenType::False),
      "for" => Some(TokenType::For),
      "fun" => Some(TokenType::Fun),
      "if" => Some(TokenType::If),
      "nil" => Some(TokenType::Nil),
      "or" => Some(TokenType::Or),
      "print" => Some(TokenType::Print),
      "return" => Some(TokenType::Return),
      "super" => Some(TokenType::Super),
      "this" => Some(TokenType::This),
      "true" => Some(TokenType::True),
      "var" => Some(TokenType::Var),
      "while" => Some(TokenType::While)
      _ => None
  }
}

impl Scanner {
  pub fn new(source: String) -> Scanner {
    Scanner {
      source,
      tokens: Vec<Token>::new(),
      start: 0,
      current: 0,
      line: 1,
    }
  }

  pub fn scan_tokens(&mut self)
  {

  }
}