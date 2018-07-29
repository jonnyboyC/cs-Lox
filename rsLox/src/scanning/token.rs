use scanning::token_type::{TokenType, LiteralTokenType};
use scanning::scanning_error::ScanningError;
use std::fmt;

#[derive(Debug, PartialEq)]
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
    line: u32) -> Result<Token, ScanningError>
  {
    // sanity check that we don't accidently pass a literal when we shouldn't
    match token_type.is_literal() {
      LiteralTokenType::Literal => {
        match literal {
          Some(_) => Ok(Token { token_type, lexeme, literal, line }),
          None => Err(ScanningError::TokenLiteralFound(token_type))
        } 
      }
      LiteralTokenType::Keyword => {
        match literal {
          Some(_) => Err(ScanningError::TokenLiteralFound(token_type)),
          None => Ok(Token { token_type, lexeme, literal, line })
        }
      }
    }
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
  fn token_constructor() {
    let token_type = TokenType::String;
    let lexeme = "test".to_string();
    let literal = Some(Literal::String("test".to_string()));
    let line = 10;

    let token = Token::new(token_type, lexeme, literal, line).unwrap();
    assert_eq!(line, token.line);
    assert_eq!("test".to_string(), token.lexeme);
    assert_eq!(Literal::String("test".to_string()), token.literal.unwrap());
    assert_eq!(TokenType::String, token.token_type);
  }

  #[test]
  fn invalid_token_constructor1() {
    let valid_enums: [TokenType; 3] = [
      TokenType::String, 
      TokenType::Number, 
      TokenType::Identifier
    ];

    let invalid_enums: [TokenType; 40] = [
      TokenType::LeftParen, TokenType::RightParen, 
      TokenType::LeftBrace, TokenType::RightBrace,
      TokenType::Comma, TokenType::Dot, TokenType::Minus, 
      TokenType::Plus, TokenType::SemiColon, TokenType::Slash, 
      TokenType::Star, TokenType::Question, TokenType::Colon,
      TokenType::Bang, TokenType::BangEqual,
      TokenType::Equal, TokenType::EqualEqual,
      TokenType::Greater, TokenType::GreaterEqual,
      TokenType::Less, TokenType::LessEqual,
      TokenType::And, TokenType::Break, TokenType::Class, 
      TokenType::Continue, TokenType::Else, TokenType::False, 
      TokenType::Fun, TokenType::For, TokenType::If, 
      TokenType::Nil, TokenType::Or, TokenType::Print, 
      TokenType::Return, TokenType::Super, TokenType::This,
      TokenType::True, TokenType::Var, TokenType::While,
      TokenType::Eof
    ];

    let line = 10;
    for valid_enum in valid_enums.clone().into_iter() {
      let lexeme = "test".to_string();
      let literal = Some(Literal::String("test".to_string()));

      assert!(Token::new(valid_enum.clone(), lexeme, literal, line).is_ok());

      let lexeme = "test".to_string();
      assert!(Token::new(valid_enum.clone(), lexeme, None, line).is_err());
    }

    for invalid_enum in invalid_enums.clone().into_iter() {
      let lexeme = "test".to_string();

      assert!(Token::new(invalid_enum.clone(), lexeme, None, line).is_ok());

      let lexeme = "test".to_string();
      let literal = Some(Literal::String("test".to_string()));
      assert!(Token::new(invalid_enum.clone(), lexeme, literal, line).is_err());
    }
  }
}

