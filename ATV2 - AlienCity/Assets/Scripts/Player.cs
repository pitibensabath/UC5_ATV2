using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Player : MonoBehaviour {

    public float speed = 5f; //Velocidade do personagem
    public float jumpForce = 500f; //Força do pulo

    public Transform PontoTiro; //De onde sairá o tiro
    public GameObject Bullet; //O que será disparado
    public GameObject Bullet2; //Campo para a segunda bala

    private Animator anim;
    private Rigidbody2D rb2d;
    private bool facingRight = true; //Verificar para qual lado o personagem está virado e alterar o lado, caso necessário
    private bool jump;
    private bool onGround = false; //Verificar se está no chão, para poder pular
    private Transform groundCheck;
    private float hForce = 0; //Pode ter três valores: -1,0,1. Dependendo do valor, irá para esquerda, direita ou ficará parado.

    public AudioClip shootSound1; //Para os áudios dos tiros
    public AudioClip shootSound2;

    public Image B1; //Para as imagens no HUD
    public Image B2;

   private bool isDead = false;

	// Use this for initialization
	void Start () {

        rb2d = GetComponent<Rigidbody2D>();
        groundCheck = gameObject.transform.Find("GroundCheck");
        //Referência do rigid body do player e do groundcheck criado para o/no player.

        anim = GetComponent<Animator>();

        Bullet.SetActive(true); //Jogo inicia com a primeira bala ativa
        Bullet2.SetActive(false); //Segunda bala em "stand by"

        B1.enabled = true; //Imagem que aparecerá incialmente no HUD, condizendo com a bala/arma inicial do jogador
        B2.enabled = false; //Segunda imagem em "stand by"
    }
	
	// Update is called once per frame
	void Update () {
		if (!isDead)
        {
            onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
            /*Para checar se o personagem está de fato no chão. É uma "linha" que sai da posição do player e vai até a
            posição do groundcheck. Essa linha colidindo com o groundcheck torna o 'onground' verdadeiro e permite, assim,
            o pulo.*/

            if (Input.GetKeyDown(KeyCode.Q)) 
            {
                mudarArma();
            }

            if (Input.GetButtonDown("Jump") && onGround)
            {
                jump = true;
            }

            if (Input.GetButtonDown("Fire1") && onGround && Bullet.activeSelf) 
             /*Ao iniciar a ação do tiro, o parametro do tiro no Animator torna-se verdadeira dando inicio à animação.
             Atirará a bala que estiver ativa no momento.*/
            {
                anim.SetTrigger("Shoot");

                //Para instanciar a bala e onde acontecerá
                GameObject mBullet = Instantiate(Bullet, PontoTiro.position, PontoTiro.rotation);

                SoundManager.instance.RandomizeSfx(shootSound1); //Som que tocará 

                mBullet.GetComponent<Renderer>().sortingLayerName = "Player"; //Layer em que aparecerá 

            } else if (Input.GetButtonDown("Fire1") && onGround && Bullet2.activeSelf)
            {
                
                anim.SetTrigger("Shoot");

                GameObject mBullet = Instantiate(Bullet2, PontoTiro.position, PontoTiro.rotation);

                SoundManager.instance.RandomizeSfx(shootSound2);

                mBullet.GetComponent<Renderer>().sortingLayerName = "Player";
            }
        }
	}

    private void FixedUpdate()
    {
        if (!isDead)
        {
            /*Para usar setas direcionais para movimentar o player. Seta para direita dá o valor 1, seta
            para a esquerda, -1, e se ficar parado, 0.*/
            hForce = Input.GetAxisRaw("Horizontal");

            /*Quando tiver algum valor atribuído ao hForce (de acordo com a seta direcional apertada ou não),
            a função Mathf.Abs vai dar o valor absoluto e passará o mesmo para o "Speed", no Animator, e 
            começará a animação*/
            anim.SetFloat("Speed", Mathf.Abs(hForce));

            /*Velocidade no eixo horizontal. O valor de hForce deve ser multiplicado pela velocidade declarada.
            A movimentação ocorrerá apenas no eixo x, logo, o eixo y permanecerá com a mesma velocidade.*/
            rb2d.velocity = new Vector2(hForce * speed, rb2d.velocity.y);

            if (jump)
            {

                anim.SetTrigger("Pula");
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
            }

            if (hForce > 0 && !facingRight)
            {
                Flip();
            }
            else if (hForce < 0 && facingRight)
            {
                Flip();
            }

            if (jump)
            {
                /*A bool jump precisa ser setada para 'false' para que o personagem não fique pulando infinitamente. Em
               seguida, adiciona uma força ao rigidbody para que ele posso realizar o movimento. Já para o Jump, quando 
               ele for verdadeiro, o mesmo será válido também dentro do Animator*/
                anim.SetBool("Jump", true);
                jump = false;
                rb2d.AddForce(Vector2.up * jumpForce); 
            }
        }
    }

    void mudarArma() //Para fazer a troca entre as balas/armas de fato
    {
        if (Bullet.activeSelf)
        {
            Bullet.SetActive(false); //Mudar de arma
            Bullet2.SetActive(true);

            B1.enabled = false; //Fazer aparecer ou tirar do HUD
            B2.enabled = true;

        } else {
            Bullet.SetActive(true);
            Bullet2.SetActive(false);

            B1.enabled = true;
            B2.enabled = false;
        }
    }

    void Flip()
    {
        /*O valor de facingRight deve ser invertido. Para isso, é só criar uma variável de escala que receberá
         o valor atual. O x da escala atual deve ser multiplicado por -1, e em seguida é só atualizar a escala
         do objeto.*/

        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

}
