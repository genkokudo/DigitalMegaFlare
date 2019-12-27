

class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            text: 'Hello Work!'
        }
    }

    render() {
        return (
            <div>
                <p> {this.state.text} </p>
            </div>
        )
    }
}

ReactDOM.render(<App />, $('#doodle').get(0));


class HelloWork extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            text: 'Hello Work!'
        }
    }

    render() {
        //// 
        //return (
        //    <div>
        //        Hello Work {this.props.name}
        //    </div>
        //)
        // 動く
        return <div>Hello Work {this.props.name}</div>;
        //// 改行すると動かない（謎）
        //return
        //<div>Hello Work {this.props.name}</div>;
    }
}

const hwElement = <HelloWork name="01" />;
ReactDOM.render(hwElement, $('#app01').get(0));

const hwFactory = React.createFactory(HelloWork);
ReactDOM.render(hwFactory({ name: '02' }), $('#app02').get(0));

// 動作確認
alert("正常です");