use scanning::tokentype::{TokenType, LiteralTokenType};
use std::fmt;

#[derive(Debug)]
pub enum Literal {
    Number(f64),
    String(String)
}

#[derive(Debug)]
pub struct Token {
  pub token_type: TokenType,
  pub lexeme: String,
  pub literal: Option<Literal>,
  pub line: u32
}


impl Token {
  pub fn new (
    token_type: TokenType, 
    lexeme: String, 
    literal: Option<Literal>,
    line: u32) -> Token
  {
    // sanity check that we don't accidently pass a literal when we shouldn't
    match TokenType.is_literal() {
      LiteralTokenType::Literal => debug_assert!(literal.is_some()),
      LiteralTokenType::Identifier => debug_assert!(literal.is_none()),
    }
    debug_assert!();
    Token { token_type, lexeme, literal, line }
  }
}

impl fmt::Display for Token {
  fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
    match self.literal
    {
      Some(ref literal) => match literal {
        Literal::Number(number) => write!(f, "{} {} {}", self.token_type, self.lexeme, number),
        Literal::String(string) => write!(f, "{} {} {}", self.token_type, self.lexeme, string),
      },
      None => write!(f, "{} {}", self.token_type, self.lexeme)
    }
  }
}



#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn vec2_constructor() {
        let token_type = TokenType::SemiColon;
        let lexeme = "test";
        let literal = Some(Literal::String("test"));
        let line = 10;

        let token = Token::New(token_type, lexeme, literal, line)

        let a: Vec2<f64> = Vec2::new(x, y);
        assert_eq!(a.x, x);
        assert_eq!(a.y, y);
    }

    #[test]
    fn vec2_add() {
        let a = Vec2::new(0.0, 5.0);
        let b = Vec2::new(10.0, 15.0);

        assert_eq!(Vec2 {x: 10.0, y: 20.0}, a + b);
    }

    #[test]
    fn vec2_display() {
        let a = Vec2::new(0.0, 10.0);

        assert_eq!(a.to_string(), "(0.00, 10.00)")
    }
}

