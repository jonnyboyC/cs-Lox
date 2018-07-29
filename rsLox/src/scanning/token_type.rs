use std::fmt;

#[derive(Debug, PartialEq, Clone)]
pub enum TokenType {
  // Single-character tokens
  LeftParen, RightParen, LeftBrace, RightBrace,
  Comma, Dot, Minus, Plus, SemiColon, Slash, Star,
  Question, Colon,

  // One or two character tokens.
  Bang, BangEqual,
  Equal, EqualEqual,
  Greater, GreaterEqual,
  Less, LessEqual,

  // Literals
  Identifier, String, Number,

  // Keywords
  And, Break, Class, Continue, Else, False, Fun,
  For, If, Nil, Or, Print, Return, Super, This,
  True, Var, While,

  Eof
}

impl TokenType {
  pub fn is_literal(&self) -> LiteralTokenType {
    match self {
      TokenType::Identifier => LiteralTokenType::Literal,
      TokenType::String => LiteralTokenType::Literal,
      TokenType::Number => LiteralTokenType::Literal,
      _ => LiteralTokenType::Keyword
    }
  }
}

#[derive(Debug, PartialEq)]
pub enum LiteralTokenType {
  Literal,
  Keyword
}

impl fmt::Display for TokenType {
  fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
    match self {
        TokenType::LeftParen => write!(f, "'('"),
        TokenType::RightParen => write!(f, "'),'"),
        TokenType::LeftBrace => write!(f, "'{{'"),
        TokenType::RightBrace => write!(f, "'}}'"),
        TokenType::Comma => write!(f, "','"),
        TokenType::Dot => write!(f, "'.'"),
        TokenType::Minus => write!(f, "'-'"),
        TokenType::Plus => write!(f, "'+'"),
        TokenType::Slash => write!(f, "'\'"),
        TokenType::Star => write!(f, "'*'"),
        TokenType::Question => write!(f, "'?'"),
        TokenType::Colon => write!(f, "':'"),
        TokenType::SemiColon => write!(f, "';'"),

        TokenType::Bang => write!(f, "'!'"),
        TokenType::BangEqual => write!(f, "'!='"),
        TokenType::Equal => write!(f, "'='"),
        TokenType::EqualEqual => write!(f, "'=='"),
        TokenType::Greater => write!(f, "'>'"),
        TokenType::GreaterEqual => write!(f, "'>='"),
        TokenType::Less => write!(f, "'<'"),
        TokenType::LessEqual => write!(f, "'<='"),

        TokenType::Identifier => write!(f, "'id'"),
        TokenType::String => write!(f, "'\"literal\"'"),
        TokenType::Number => write!(f, "'0..9'"),

        TokenType::And => write!(f, "'and'"),
        TokenType::Break => write!(f, "'break'"),
        TokenType::Class => write!(f, "'class'"),
        TokenType::Continue => write!(f, "'continue'"),
        TokenType::Else => write!(f, "'else'"),
        TokenType::False => write!(f, "'false'"),
        TokenType::Fun => write!(f, "'fun'"),
        TokenType::For => write!(f, "'for'"),
        TokenType::If => write!(f, "'if'"),
        TokenType::Nil => write!(f, "'nil'"),
        TokenType::Or => write!(f, "'or'"),
        TokenType::Print => write!(f, "'print'"),
        TokenType::Return => write!(f, "'return'"),
        TokenType::Super => write!(f, "'super'"),
        TokenType::This => write!(f, "'this'"),
        TokenType::True => write!(f, "'true'"),
        TokenType::Var => write!(f, "'var'"),
        TokenType::While => write!(f, "'while'"),

        TokenType::Eof => write!(f, "'eof'"),
    }
  }
}