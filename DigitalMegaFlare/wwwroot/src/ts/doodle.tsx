// 何で別でReactをCDNして、こっちをコメントアウトすればいいのかわからない。一切インポートできないってこと？
//import * as React from "react";
//import * as ReactDOM from "react-dom";

class Hello extends React.Component<{
    /**
     * @default 'Hello Work!'
     */
    compiler?: string,
    framework: string
}> {
    static defaultProps = {
        compiler: 'Hello Work!'
    }
    render() {
        const compiler = this.props.compiler!;
        return (
            <div>
                <div>{compiler}</div>
                <div>{this.props.framework}</div>
            </div>
        );
    }
}

const hwElement = <Hello framework="React" />;
ReactDOM.render(hwElement, document.getElementById('app01'));

// Factoryを作ってから描画する方法
const hwFactory = React.createFactory(Hello);
ReactDOM.render(hwFactory({ framework: 'ananan' }), document.getElementById('app02'));





// 動作確認
alert("正常です");
