// 何で別でReactをCDNして、こっちをコメントアウトすればいいのかわからない。一切インポートできないってこと？
//import * as React from "react";
//import * as ReactDOM from "react-dom";

class App extends React.Component {
    constructor(props: string) {
        super(props);
        this.state = {
            text: 'Hello Work!'
        }
    }

    render() {
        return (
            <div>
                <p>Hello Work!</p>
            </div>
        )
    }
}

// 何でエラーが出るのかわからない
// TSでReact書くための基礎を調べること
//<p>{this.state.text} </p>

//ReactDOM.render(<App />, 'aaaa');
ReactDOM.render(<App />, document.getElementById('doodle'));
                    
class HelloWork extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            text: 'Hello Work!'
        }
    }

    render() {
        //return <div>Hello Work {this.props.name} </div>;
        return <div>Hello Work !! </div>;
    }
}

// 動的に変数を入れることができないのは何で？
//const hwElement = <HelloWork name="01"/>;
//ReactDOM.render(hwElement, document.getElementById('app01'));
                    
const hwFactory = React.createFactory(HelloWork);
ReactDOM.render(hwFactory({ name: '02' }), document.getElementById('app02'));
                    
// 動作確認
alert("正常です");
